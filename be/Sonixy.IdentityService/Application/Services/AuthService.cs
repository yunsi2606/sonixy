using Microsoft.Extensions.Options;
using Sonixy.IdentityService.Application.DTOs;
using Sonixy.IdentityService.Domain.Entities;
using Sonixy.IdentityService.Domain.Repositories;
using Sonixy.Shared.Configuration;
using Sonixy.Shared.Protos;

namespace Sonixy.IdentityService.Application.Services;

public class AuthService(
    IAccountRepository accountRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    UserService.UserServiceClient userServiceClient,
    IOptions<JwtSettings> jwtSettings) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        // Check if email already exists
        if (await accountRepository.EmailExistsAsync(dto.Email, cancellationToken))
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Create new account
        var account = new Account
        {
            Email = dto.Email.ToLowerInvariant(),
            PasswordHash = passwordHasher.HashPassword(dto.Password),
            IsEmailVerified = false,
            IsActive = true
        };

        await accountRepository.AddAsync(account, cancellationToken);
        
        // Sync with User Service
        try 
        {
            await userServiceClient.CreateUserAsync(new Shared.Protos.CreateUserRequest
            {
                Id = account.Id.ToString(),
                Email = dto.Email,
                Username = dto.Username,
                FirstName = "",
                LastName = ""
            }, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Account created but failed to create user profile: {ex.Message}", ex);
        }

        // Generate tokens
        return await GenerateAuthResponseAsync(account, cancellationToken);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        // Get account by email
        var account = await accountRepository.GetByEmailAsync(dto.Email, cancellationToken);
        
        if (account is null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password
        if (!passwordHasher.VerifyPassword(dto.Password, account.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if account is active
        if (!account.IsActive)
        {
            throw new UnauthorizedAccessException("Account is disabled");
        }

        // Update last login
        account.LastLoginAt = DateTime.UtcNow;
        await accountRepository.UpdateAsync(account, cancellationToken);

        // Generate tokens
        return await GenerateAuthResponseAsync(account, cancellationToken);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // Get refresh token from database
        var token = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

        if (token is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Check if token is revoked
        if (token.IsRevoked)
        {
            throw new UnauthorizedAccessException("Token has been revoked");
        }

        // Check if token is expired
        if (token.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token has expired");
        }

        // Get account
        var account = await accountRepository.GetByIdAsync(token.AccountId, cancellationToken);
        
        if (account is null || !account.IsActive)
        {
            throw new UnauthorizedAccessException("Account not found or inactive");
        }

        // Revoke old token
        await refreshTokenRepository.RevokeTokenAsync(refreshToken, "Replaced by new token", cancellationToken);

        // Generate new tokens
        return await GenerateAuthResponseAsync(account, cancellationToken);
    }

    public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        await refreshTokenRepository.RevokeTokenAsync(refreshToken, "Revoked by user", cancellationToken);
    }

    public Task<bool> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var principal = tokenService.ValidateToken(accessToken);
        return Task.FromResult(principal is not null);
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(Account account, CancellationToken cancellationToken)
    {
        var userId = account.Id.ToString();
        
        // Generate access token
        var accessToken = tokenService.GenerateAccessToken(userId, account.Email);

        // Generate refresh token
        var refreshTokenString = tokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        // Save refresh token to database
        var refreshToken = new RefreshToken
        {
            AccountId = account.Id,
            Token = refreshTokenString,
            ExpiresAt = refreshTokenExpiry,
            IsRevoked = false
        };

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return new AuthResponseDto(
            accessToken,
            refreshTokenString,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            userId
        );
    }
}

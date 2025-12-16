using Sonixy.IdentityService.Application.DTOs;

namespace Sonixy.IdentityService.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}

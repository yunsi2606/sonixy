using Microsoft.AspNetCore.Mvc;
using Sonixy.IdentityService.Application.DTOs;
using Sonixy.IdentityService.Application.Services;

namespace Sonixy.IdentityService.Api.Controllers;

/// <summary>
/// Authentication endpoints for user registration, login, and token management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <remarks>
    /// Creates a new account with email and password. Returns access and refresh tokens.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/auth/register
    ///     {
    ///       "email": "user@example.com",
    ///       "password": "SecurePass123!",
    ///       "displayName": "Nhat Cuong"
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto">Registration data</param>
    /// <returns>Authentication response with tokens</returns>
    /// <response code="200">Registration successful</response>
    /// <response code="400">Invalid request or email already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var response = await authService.RegisterAsync(dto);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <remarks>
    /// Authenticates user and returns access and refresh tokens.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///       "email": "user@example.com",
    ///       "password": "SecurePass123!"
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto">Login credentials</param>
    /// <returns>Authentication response with tokens</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials or account disabled</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var response = await authService.LoginAsync(dto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <remarks>
    /// Generates a new access token and refresh token pair.
    /// The old refresh token will be revoked.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/auth/refresh
    ///     {
    ///       "refreshToken": "your_refresh_token_here"
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto">Refresh token</param>
    /// <returns>New authentication response with tokens</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        try
        {
            var response = await authService.RefreshTokenAsync(dto.RefreshToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Revoke a refresh token (logout)
    /// </summary>
    /// <remarks>
    /// Invalidates the refresh token, effectively logging out the user.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/auth/revoke
    ///     {
    ///       "refreshToken": "your_refresh_token_here"
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto">Refresh token to revoke</param>
    /// <returns>Success message</returns>
    /// <response code="200">Token revoked successfully</response>
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto dto)
    {
        await authService.RevokeTokenAsync(dto.RefreshToken);
        return Ok(new { message = "Token revoked successfully" });
    }

    /// <summary>
    /// Verify email with token
    /// </summary>
    /// <param name="token">Verification token</param>
    /// <returns>Success message</returns>
    [HttpPost("verify-email")]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        try
        {
            await authService.VerifyEmailAsync(token);
            return Ok(new { message = "Email verified successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Validate access token
    /// </summary>
    /// <remarks>
    /// Checks if an access token is valid and not expired.
    /// Used for token verification in other services.
    /// </remarks>
    /// <param name="token">JWT access token</param>
    /// <returns>Validation result</returns>
    /// <response code="200">Token validation result</response>
    [HttpGet("validate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateToken([FromQuery] string token)
    {
        var isValid = await authService.ValidateTokenAsync(token);
        return Ok(new { isValid });
    }
}

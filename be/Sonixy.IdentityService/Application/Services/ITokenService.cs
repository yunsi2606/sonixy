using System.Security.Claims;

namespace Sonixy.IdentityService.Application.Services;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string email);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    string? GetUserIdFromToken(string token);
}

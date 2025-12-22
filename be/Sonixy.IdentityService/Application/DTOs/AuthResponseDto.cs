namespace Sonixy.IdentityService.Application.DTOs;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string UserId,
    bool IsEmailVerified
);

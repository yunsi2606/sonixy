namespace Sonixy.UserService.Application.DTOs;

public record UserDto(
    string Id,
    string DisplayName,
    string Email,
    string Bio,
    string AvatarUrl,
    DateTime CreatedAt
);

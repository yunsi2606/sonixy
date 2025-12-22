namespace Sonixy.UserService.Application.DTOs;

public record UserDto(
    string Id,
    string? FirstName,
    string? LastName,
    string DisplayName,
    string Email,
    string Username,
    string Bio,
    string AvatarUrl,
    DateTime CreatedAt,
    bool IsEmailVerified
);

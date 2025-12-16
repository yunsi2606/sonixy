using System.ComponentModel.DataAnnotations;

namespace Sonixy.UserService.Application.DTOs;

public record CreateUserDto(
    [Required][MaxLength(50)] string DisplayName,
    [Required][EmailAddress] string Email,
    [MaxLength(500)] string? Bio,
    string? AvatarUrl
);

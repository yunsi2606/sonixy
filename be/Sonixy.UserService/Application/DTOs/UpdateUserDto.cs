using System.ComponentModel.DataAnnotations;

namespace Sonixy.UserService.Application.DTOs;

public record UpdateUserDto(
    [MaxLength(50)] string? DisplayName,
    [MaxLength(500)] string? Bio,
    string? AvatarUrl
);

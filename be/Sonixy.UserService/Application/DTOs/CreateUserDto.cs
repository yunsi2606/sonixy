using System.ComponentModel.DataAnnotations;

namespace Sonixy.UserService.Application.DTOs;

public record CreateUserDto(
    [MaxLength(50)] string? FirstName,
    [MaxLength(50)] string? LastName,
    [Required][EmailAddress] string Email,
    [Required] string Username,
    [MaxLength(500)] string? Bio,
    string? AvatarUrl,
    string? Id = null
);

using System.ComponentModel.DataAnnotations;

namespace Sonixy.IdentityService.Application.DTOs;

public record RegisterDto(
    [Required][EmailAddress] string Email,
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "Username must be 3-20 characters long and contain only letters, numbers, and underscores.")]
    string Username,
    [Required][MinLength(8)] string Password
);

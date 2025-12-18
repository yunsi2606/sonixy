using System.ComponentModel.DataAnnotations;

namespace Sonixy.IdentityService.Application.DTOs;

public record RegisterDto(
    [Required][EmailAddress] string Email,
    [Required][MinLength(8)] string Password
);

using System.ComponentModel.DataAnnotations;

namespace Sonixy.IdentityService.Application.DTOs;

public record LoginDto(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

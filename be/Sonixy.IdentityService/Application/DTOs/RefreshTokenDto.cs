using System.ComponentModel.DataAnnotations;

namespace Sonixy.IdentityService.Application.DTOs;

public record RefreshTokenDto(
    [Required] string RefreshToken
);

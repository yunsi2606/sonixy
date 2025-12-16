using System.ComponentModel.DataAnnotations;

namespace Sonixy.PostService.Application.DTOs;

public record CreatePostDto(
    [Required][MaxLength(1000)] string Content,
    [Required] string Visibility = "public" // "public" | "followers"
);

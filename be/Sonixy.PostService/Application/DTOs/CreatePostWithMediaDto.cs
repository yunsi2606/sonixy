using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sonixy.PostService.Application.DTOs;

public class CreatePostWithMediaDto
{
    [Required] 
    [MaxLength(1000)] 
    public string Content { get; set; } = string.Empty;

    [Required] 
    public string Visibility { get; set; } = "public";

    public List<IFormFile>? Media { get; set; }
}

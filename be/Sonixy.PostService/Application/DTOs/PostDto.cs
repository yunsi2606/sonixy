namespace Sonixy.PostService.Application.DTOs;

public record PostDto(
    string Id,
    string AuthorId,
    string Content,
    string Visibility,
    int LikeCount,
    bool IsLiked,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

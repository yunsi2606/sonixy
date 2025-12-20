namespace Sonixy.PostService.Application.DTOs;

public record PostDto(
    string Id,
    string AuthorId,
    string AuthorName,
    string AuthorAvatarUrl,
    string Content,
    string Visibility,
    int LikeCount,
    bool IsLiked,
    List<MediaItemDto> Media,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record MediaItemDto(string Type, string Url);

namespace Sonixy.PostService.Application.DTOs;

public record PostDto(
    string Id,
    string AuthorId,
    string AuthorDisplayName,
    string AuthorAvatarUrl,
    string AuthorUsername,
    string Content,
    string Visibility,
    int LikeCount,
    bool IsLiked,
    List<MediaItemDto> Media,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record MediaItemDto(string Type, string Url);

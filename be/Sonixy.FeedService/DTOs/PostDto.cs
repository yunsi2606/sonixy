namespace Sonixy.FeedService.DTOs;

public record PostDto(
    string Id,
    string AuthorId,
    string AuthorDisplayName,
    string AuthorAvatarUrl,
    string AuthorUsername,
    string Content,
    string Visibility,
    long LikeCount,
    bool IsLiked,
    List<MediaItemDto> Media,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record MediaItemDto(string Type, string Url);

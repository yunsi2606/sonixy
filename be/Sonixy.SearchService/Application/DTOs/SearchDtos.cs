namespace Sonixy.SearchService.Application.DTOs;

public record UserSearchResultDto(
    string Id,
    string Username,
    string DisplayName,
    string AvatarUrl,
    string Bio
);

public record PostSearchResultDto(
    string Id,
    string AuthorId,
    string AuthorUsername,
    string AuthorDisplayName,
    string AuthorAvatarUrl,
    string Content,
    List<string> Hashtags,
    DateTime CreatedAt
);

public record SearchResultDto(
    List<UserSearchResultDto> Users,
    List<PostSearchResultDto> Posts
);

public record TrendingHashtagDto(
    string Tag,
    int Count
);

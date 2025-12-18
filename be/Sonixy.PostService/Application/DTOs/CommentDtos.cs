namespace Sonixy.PostService.Application.DTOs;

public record CreateCommentDto(
    string PostId,
    string Content,
    string AuthorUsername, 
    string AuthorAvatarUrl,
    string? ParentId,
    string? ReplyToUserId,
    string? ReplyToUsername
);

public record CommentDto(
    string Id,
    string PostId,
    string Content,
    UserSimpleDto Author,
    string CreatedAt,
    string? ParentId,
    ReplyToDto? ReplyTo,
    int Likes,
    bool IsLiked
);

public record UserSimpleDto(string Id, string Username, string AvatarUrl);
public record ReplyToDto(string UserId, string Username);

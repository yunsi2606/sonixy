namespace Sonixy.Shared.Events;

public record PostCreatedEvent(
    string PostId,
    string AuthorId,
    string Content,
    List<string> ImageUrls,
    DateTime CreatedAt
);

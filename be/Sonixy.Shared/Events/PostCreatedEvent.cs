namespace Sonixy.Shared.Events;

public record PostCreatedEvent(
    string PostId,
    string AuthorId,
    string Content,
    List<string> ImageUrls,
    List<string> Hashtags,
    DateTime CreatedAt
)
{
    private PostCreatedEvent() : this(default!, default!, default!, [], [], default) { }
}

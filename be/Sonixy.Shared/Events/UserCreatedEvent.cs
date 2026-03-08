namespace Sonixy.Shared.Events;

/// <summary>
/// Event published when a new user account is created
/// </summary>
public record UserCreatedEvent(
    string UserId,
    string Username,
    string DisplayName,
    string Email,
    string AvatarUrl,
    string Bio,
    DateTime CreatedAt
)
{
    private UserCreatedEvent() : this(default!, default!, default!, default!, default!, default!, default) { }
}

namespace Sonixy.Shared.Events;

public enum UserActionType
{
    View,   // Passive View (dwell time > threshold)
    Click,  // Click to open details
    Like,   // Explicit Like
    Share,  // Share post
    Comment, // Add comment
    Scroll, // Scroll past (Negative signal if fast)
    ProfileView // Visit User Profile
}

public enum TargetType
{
    Post,
    User,
    Comment
}

public record UserInteractionEvent(
    string UserId,
    string TargetId,
    TargetType TargetType,
    UserActionType ActionType,
    int DurationMs = 0,
    DateTime Timestamp = default
);

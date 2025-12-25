namespace Sonixy.Shared.Events;

public enum UserActionType
{
    View,   // Passive View (dwell time > threshold)
    Click,  // Click to open details
    Like,   // Explicit Like
    Share,  // Share post
    Comment, // Add comment
    Scroll, // Scroll past (Negative signal if fast)
    ProfileView, // Visit User Profile
    Reply   // Reply to a comment
}

public enum TargetType
{
    Post,
    User,
    Comment
}

public record UserInteractionEvent(
    string UserId,           // The actor (required)
    string TargetId,         // The entity acted upon (required)
    TargetType TargetType,   // Enum (required for analytics)
    UserActionType ActionType, // Enum (required for analytics)
    int DurationMs = 0,      // For View/Scroll events
    DateTime Timestamp = default, // Event time
    
    // Notification Enrichment Fields (Optional/Nullable)
    string? ActorName = null,
    string? ActorAvatar = null,
    string? TargetUserId = null // Recipient of notification
);

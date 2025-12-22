namespace Sonixy.Shared.Events;

public record EmailVerificationRequestedEvent(
    string UserId,
    string Email,
    string Token,
    DateTime ExpiresAt
);

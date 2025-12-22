namespace Sonixy.Shared.Events;

public record EmailVerifiedEvent(string UserId, string Email, DateTime VerifiedAt);

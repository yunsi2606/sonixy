using System;
using Sonixy.NotificationService.Domain.Enums;
using Sonixy.Shared.Common;

namespace Sonixy.NotificationService.Domain.Entities;

public class Notification: Entity
{
    public string RecipientId { get; set; } = null!;
    public string ActorId { get; set; } = null!;
    public string ActorName { get; set; } = null!;
    public string ActorAvatar { get; set; } = null!; // Optional, can be fetched if needed, but good for caching
    
    public string EntityId { get; set; } = null!; // PostId, CommentId, etc.
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
}

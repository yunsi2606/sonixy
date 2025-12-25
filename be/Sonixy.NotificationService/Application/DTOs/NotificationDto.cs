using System;
using Sonixy.NotificationService.Domain.Enums;

namespace Sonixy.NotificationService.Application.DTOs;

public class NotificationDto
{
    public string Id { get; set; } = null!;
    public string RecipientId { get; set; } = null!;
    public string ActorId { get; set; } = null!;
    public string ActorName { get; set; } = null!;
    public string ActorAvatar { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateNotificationDto
{
    public string RecipientId { get; set; } = null!;
    public string ActorId { get; set; } = null!;
    public string ActorName { get; set; } = null!;
    public string ActorAvatar { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public NotificationType Type { get; set; }
}

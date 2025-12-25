using MassTransit;
using Sonixy.Shared.Events;
using Sonixy.NotificationService.Domain.Entities;
using Sonixy.NotificationService.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Sonixy.NotificationService.DTOs;
using Sonixy.NotificationService.Hubs;

namespace Sonixy.NotificationService.Consumers;

public class UserInteractionConsumer(
    INotificationRepository repository,
    IHubContext<NotificationHub> hubContext) : IConsumer<UserInteractionEvent>
{
    public async Task Consume(ConsumeContext<UserInteractionEvent> context)
    {
        var evt = context.Message;
        
        // Validation: Must have a recipient to notify
        if (string.IsNullOrEmpty(evt.TargetUserId)) return;
        
        // Don't notify self (Actor == Recipient)
        if (evt.UserId == evt.TargetUserId) return;

        // Map Event to Notification Entity
        var notification = new Notification
        {
            RecipientId = evt.TargetUserId,
            ActorId = evt.UserId,
            ActorName = evt.ActorName ?? "Unknown",
            ActorAvatar = evt.ActorAvatar ?? "",
            EntityId = evt.TargetId,
            EntityType = evt.TargetType.ToString(), // "Post", "Comment"
            Action = evt.ActionType.ToString(),     // "Like", "Comment", "Reply"
            IsRead = false,
            CreatedAt = evt.Timestamp == default ? DateTime.UtcNow : evt.Timestamp
        };

        // Persist
        await repository.AddAsync(notification);

        // Real-time Push (SignalR)
        // Push to the Group named by TargetUserId (the recipient)
        var dto = new NotificationDto(
            notification.Id.ToString(),
            notification.ActorId,
            notification.ActorName,
            notification.ActorAvatar,
            notification.EntityId,
            notification.EntityType,
            notification.Action, 
            notification.IsRead,
            notification.CreatedAt
        );

        await hubContext.Clients.Group(notification.RecipientId).SendAsync("ReceiveNotification", dto);
    }
}

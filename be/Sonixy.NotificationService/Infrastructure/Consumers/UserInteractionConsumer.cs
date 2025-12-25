using MassTransit;
using Sonixy.NotificationService.Application.DTOs;
using Sonixy.NotificationService.Application.Services;
using Sonixy.NotificationService.Domain.Enums;
using Sonixy.Shared.Events;
using System.Threading.Tasks;

namespace Sonixy.NotificationService.Infrastructure.Consumers;

public class UserInteractionConsumer(INotificationService service) : IConsumer<UserInteractionEvent>
{
    public async Task Consume(ConsumeContext<UserInteractionEvent> context)
    {
        var evt = context.Message;
        
        // Validation: Must have a recipient to notify
        if (string.IsNullOrEmpty(evt.TargetUserId)) return;
        
        // Don't notify self (Actor == Recipient)
        if (evt.UserId == evt.TargetUserId) return;

        // Map Event ActionType to NotificationType
        var type = evt.ActionType switch
        {
            UserActionType.Like => NotificationType.Like,
            UserActionType.Comment => NotificationType.Comment,
            UserActionType.Reply => NotificationType.Reply,
            _ => NotificationType.Like // Default fallback
        };

        // Create Notification via Service (which handles Persistence + SignalR)
        var dto = new CreateNotificationDto
        {
            RecipientId = evt.TargetUserId,
            ActorId = evt.UserId,
            ActorName = evt.ActorName ?? "Unknown",
            ActorAvatar = evt.ActorAvatar ?? "",
            EntityId = evt.TargetId,
            Type = type
        };

        await service.CreateNotificationAsync(dto);
    }
}

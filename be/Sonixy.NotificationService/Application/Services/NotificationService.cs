using Sonixy.NotificationService.Application.DTOs;
using Sonixy.NotificationService.Application.Interfaces;
using Sonixy.NotificationService.Domain.Entities;
using Sonixy.NotificationService.Domain.Enums;
using Sonixy.NotificationService.Domain.Repositories;

namespace Sonixy.NotificationService.Application.Services;

public class NotificationService(INotificationRepository repository, INotifier notifier) : INotificationService
{
    private readonly INotificationRepository _repository = repository;

    public async Task CreateNotificationAsync(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            RecipientId = dto.RecipientId,
            ActorId = dto.ActorId,
            ActorName = dto.ActorName,
            ActorAvatar = dto.ActorAvatar,
            EntityId = dto.EntityId,
            Type = dto.Type,
            Status = NotificationStatus.Unread,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(notification);

        // Real-time notification
        var notificationDto = new NotificationDto
        {
            Id = notification.Id.ToString(),
            RecipientId = notification.RecipientId,
            ActorId = notification.ActorId,
            ActorName = notification.ActorName,
            ActorAvatar = notification.ActorAvatar,
            EntityId = notification.EntityId,
            Type = notification.Type,
            Status = notification.Status,
            CreatedAt = notification.CreatedAt
        };
        await notifier.NotifyAsync(notification.RecipientId, notificationDto);
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, int page, int pageSize)
    {
        var notifications = await _repository.GetByRecipientIdAsync(userId, page, pageSize);
        
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id.ToString(),
            RecipientId = n.RecipientId,
            ActorId = n.ActorId,
            ActorName = n.ActorName,
            ActorAvatar = n.ActorAvatar,
            EntityId = n.EntityId,
            Type = n.Type,
            Status = n.Status,
            CreatedAt = n.CreatedAt
        });
    }

    public async Task<long> GetUnreadCountAsync(string userId)
    {
        return await _repository.GetUnreadCountAsync(userId);
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        await _repository.MarkAsReadAsync(notificationId);
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        await repository.MarkAllAsReadAsync(userId);
    }
}

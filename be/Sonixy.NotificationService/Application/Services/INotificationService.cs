using System.Collections.Generic;
using System.Threading.Tasks;
using Sonixy.NotificationService.Application.DTOs;

namespace Sonixy.NotificationService.Application.Services;

public interface INotificationService
{
    Task CreateNotificationAsync(CreateNotificationDto dto);
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, int page, int pageSize);
    Task<long> GetUnreadCountAsync(string userId);
    Task MarkAsReadAsync(string notificationId);
    Task MarkAllAsReadAsync(string userId);
}

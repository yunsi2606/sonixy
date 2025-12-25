using System.Collections.Generic;
using System.Threading.Tasks;
using Sonixy.NotificationService.Domain.Entities;
using Sonixy.Shared.Common;

namespace Sonixy.NotificationService.Domain.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetByRecipientIdAsync(string recipientId, int page, int pageSize);
    Task<long> GetUnreadCountAsync(string recipientId);
    Task MarkAsReadAsync(string notificationId);
    Task MarkAllAsReadAsync(string recipientId);
}

using System.Threading.Tasks;
using Sonixy.NotificationService.Application.DTOs;

namespace Sonixy.NotificationService.Application.Interfaces;

public interface INotifier
{
    Task NotifyAsync(string userId, NotificationDto notification);
}

using Microsoft.AspNetCore.SignalR;
using Sonixy.NotificationService.Application.DTOs;
using Sonixy.NotificationService.Application.Interfaces;
using Sonixy.NotificationService.Infrastructure.Hubs;
using System.Threading.Tasks;

namespace Sonixy.NotificationService.Infrastructure.Services;

public class SignalRNotifier(IHubContext<NotificationHub> hubContext) : INotifier
{
    public async Task NotifyAsync(string userId, NotificationDto notification)
    {
        await hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", notification);
    }
}

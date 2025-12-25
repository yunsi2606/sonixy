using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonixy.NotificationService.Infrastructure.Repositories;
using System.Security.Claims;

namespace Sonixy.NotificationService.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController(INotificationRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var notifications = await repository.GetUserNotificationsAsync(userId, pageIndex, pageSize);
        var unreadCount = await repository.GetUnreadCountAsync(userId);
        
        return Ok(new {
            Items = notifications,
            UnreadCount = unreadCount,
            HasMore = notifications.Count == pageSize
        });
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        await repository.MarkAsReadAsync(id);
        return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await repository.MarkAllAsReadAsync(userId);
        return NoContent();
    }
}

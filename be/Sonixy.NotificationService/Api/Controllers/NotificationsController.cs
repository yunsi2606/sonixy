using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Sonixy.NotificationService.Application.DTOs;
using Sonixy.NotificationService.Application.Services;

namespace Sonixy.NotificationService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(INotificationService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var notifications = await service.GetUserNotificationsAsync(userId, page, pageSize);
        var unreadCount = await service.GetUnreadCountAsync(userId);
        
        var list = notifications.ToList();
        
        return Ok(new NotificationListDto
        { 
            Items = list, 
            UnreadCount = unreadCount,
            HasMore = list.Count == pageSize
        });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var count = await service.GetUnreadCountAsync(userId);
        return Ok(new { Count = count });
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        await service.MarkAsReadAsync(id);
        return Ok();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await service.MarkAllAsReadAsync(userId);
        return Ok();
    }
}

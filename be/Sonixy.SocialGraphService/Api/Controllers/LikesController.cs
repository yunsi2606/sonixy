using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MassTransit;
using Sonixy.Shared.Events;
using Sonixy.SocialGraphService.Application.Services;

namespace Sonixy.SocialGraphService.Api.Controllers;

/// <summary>
/// Post like interactions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikesController(ISocialGraphService socialGraphService, IPublishEndpoint publishEndpoint)
    : Controller
{
    /// <summary>
    /// Toggle like a post
    /// </summary>
    [HttpPost("{postId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ToggleLike(string postId, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var liked = await socialGraphService.ToggleLikeAsync(userId, postId, ct);
        await publishEndpoint.Publish(new UserInteractionEvent(
            UserId: userId,
            TargetId: postId,
            TargetType: TargetType.Post,
            ActionType: UserActionType.Like,
            Timestamp: DateTime.UtcNow
        ), ct);
        
        return Ok(new { liked });
    }

    /// <summary>
    /// Get like count for a post
    /// </summary>
    [HttpGet("{postId}/count")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLikeCount(string postId)
    {
        var count = await socialGraphService.GetLikeCountAsync(postId);
        return Ok(new { count });
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Sonixy.SocialGraphService.Api.Controllers;

/// <summary>
/// Post like interactions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikesController : ControllerBase
{
    private readonly Application.Services.ISocialGraphService _socialGraphService;

    public LikesController(Application.Services.ISocialGraphService socialGraphService)
    {
        _socialGraphService = socialGraphService;
    }

    /// <summary>
    /// Like a post
    /// </summary>
    [HttpPost("{postId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LikePost(string postId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _socialGraphService.LikePostAsync(userId, postId);
        return Ok(new { message = "Post liked" });
    }

    /// <summary>
    /// Unlike a post
    /// </summary>
    [HttpDelete("{postId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnlikePost(string postId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _socialGraphService.UnlikePostAsync(userId, postId);
        return Ok(new { message = "Post unliked" });
    }

    /// <summary>
    /// Get like count for a post
    /// </summary>
    [HttpGet("{postId}/count")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLikeCount(string postId)
    {
        var count = await _socialGraphService.GetLikeCountAsync(postId);
        return Ok(new { count });
    }
}

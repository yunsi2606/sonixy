using Microsoft.AspNetCore.Mvc;

namespace Sonixy.SocialGraphService.Api.Controllers;

/// <summary>
/// Post like interactions
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> LikePost(string postId)
    {
        var userId = "507f1f77bcf86cd799439011"; // TODO: Extract from JWT
        await _socialGraphService.LikePostAsync(userId, postId);
        return Ok(new { message = "Post liked" });
    }

    /// <summary>
    /// Unlike a post
    /// </summary>
    [HttpDelete("{postId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UnlikePost(string postId)
    {
        var userId = "507f1f77bcf86cd799439011"; // TODO: Extract from JWT
        await _socialGraphService.UnlikePostAsync(userId, postId);
        return Ok(new { message = "Post unliked" });
    }

    /// <summary>
    /// Get like count for a post
    /// </summary>
    [HttpGet("{postId}/count")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLikeCount(string postId)
    {
        var count = await _socialGraphService.GetLikeCountAsync(postId);
        return Ok(new { count });
    }
}

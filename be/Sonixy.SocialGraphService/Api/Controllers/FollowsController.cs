using Microsoft.AspNetCore.Mvc;
using Sonixy.SocialGraphService.Application.Services;

namespace Sonixy.SocialGraphService.Api.Controllers;

/// <summary>
/// Social graph interactions - follow relationships
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FollowsController : ControllerBase
{
    private readonly ISocialGraphService _socialGraphService;

    public FollowsController(ISocialGraphService socialGraphService)
    {
        _socialGraphService = socialGraphService;
    }

    /// <summary>
    /// Follow a user
    /// </summary>
    /// <param name="followingId">ID of the user to follow</param>
    /// <returns>Success status</returns>
    [HttpPost("{followingId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FollowUser(string followingId)
    {
        var userId = "507f1f77bcf86cd799439011"; // TODO: Extract from JWT
        await _socialGraphService.FollowUserAsync(userId, followingId);
        return Ok(new { message = "User followed successfully" });
    }

    /// <summary>
    /// Unfollow a user
    /// </summary>
    /// <param name="followingId">ID of the user to unfollow</param>
    [HttpDelete("{followingId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UnfollowUser(string followingId)
    {
        var userId = "507f1f77bcf86cd799439011"; // TODO: Extract from JWT
        await _socialGraphService.UnfollowUserAsync(userId, followingId);
        return Ok(new { message = "User unfollowed successfully" });
    }

    /// <summary>
    /// Check if currently following a user
    /// </summary>
    [HttpGet("{followingId}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowStatus(string followingId)
    {
        var userId = "507f1f77bcf86cd799439011"; // TODO: Extract from JWT
        var isFollowing = await _socialGraphService.IsFollowingAsync(userId, followingId);
        return Ok(new { isFollowing });
    }
}

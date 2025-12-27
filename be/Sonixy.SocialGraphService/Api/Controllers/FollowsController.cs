using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonixy.SocialGraphService.Application.Services;
using System.Security.Claims;

namespace Sonixy.SocialGraphService.Api.Controllers;

/// <summary>
/// Social graph interactions - follow relationships
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> FollowUser(string followingId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _socialGraphService.FollowUserAsync(userId, followingId);
        return Ok(new { message = "User followed successfully" });
    }

    /// <summary>
    /// Unfollow a user
    /// </summary>
    /// <param name="followingId">ID of the user to unfollow</param>
    [HttpDelete("{followingId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnfollowUser(string followingId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _socialGraphService.UnfollowUserAsync(userId, followingId);
        return Ok(new { message = "User unfollowed successfully" });
    }

    /// <summary>
    /// Check if currently following a user
    /// </summary>
    [HttpGet("{followingId}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFollowStatus(string followingId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var isFollowing = await _socialGraphService.IsFollowingAsync(userId, followingId);
        return Ok(new { isFollowing });
    }

    /// <summary>
    /// Get follower count for a user
    /// </summary>
    [HttpGet("{userId}/followers/count")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowersCount(string userId)
    {
        var count = await _socialGraphService.GetFollowersCountAsync(userId);
        return Ok(new { count });
    }

    /// <summary>
    /// Get following count for a user
    /// </summary>
    [HttpGet("{userId}/following/count")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowingCount(string userId)
    {
        var count = await _socialGraphService.GetFollowingCountAsync(userId);
        return Ok(new { count });
    }

    /// <summary>
    /// Get list of followers for a user
    /// </summary>
    [HttpGet("{userId}/followers")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowers(string userId, [FromQuery] int skip = 0, [FromQuery] int limit = 20)
    {
        var followers = await _socialGraphService.GetFollowersAsync(userId, skip, limit);
        return Ok(followers);
    }

    /// <summary>
    /// Get list of users followed by a user
    /// </summary>
    [HttpGet("{userId}/following")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowing(string userId, [FromQuery] int skip = 0, [FromQuery] int limit = 20)
    {
        var following = await _socialGraphService.GetFollowingAsync(userId, skip, limit);
        return Ok(following);
    }

    /// <summary>
    /// Get mutual follows (users who follow each other)
    /// </summary>
    [HttpGet("mutuals")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMutualFollows([FromQuery] int skip = 0, [FromQuery] int limit = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var mutuals = await _socialGraphService.GetMutualFollowsAsync(userId, skip, limit);
        return Ok(mutuals);
    }

    /// <summary>
    /// Check if target user is a mutual follow
    /// </summary>
    [HttpGet("{targetId}/is-mutual")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IsMutualFollow(string targetId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var isMutual = await _socialGraphService.IsMutualFollowAsync(userId, targetId);
        return Ok(new { isMutual });
    }
}

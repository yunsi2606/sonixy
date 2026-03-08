using Microsoft.AspNetCore.Mvc;
using Sonixy.SearchService.Application.DTOs;
using Sonixy.SearchService.Application.Services;

namespace Sonixy.SearchService.Api.Controllers;

/// <summary>
/// Search endpoints for users and posts
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SearchController(ISearchService searchService) : ControllerBase
{
    /// <summary>
    /// Search users by username, display name, or bio
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(List<UserSearchResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchUsers([FromQuery] string q, [FromQuery] int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Query parameter 'q' is required" });

        limit = Math.Clamp(limit, 1, 50);
        var results = await searchService.SearchUsersAsync(q, limit);
        return Ok(results);
    }

    /// <summary>
    /// Search posts by content or hashtag
    /// </summary>
    [HttpGet("posts")]
    [ProducesResponseType(typeof(List<PostSearchResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchPosts([FromQuery] string q, [FromQuery] int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Query parameter 'q' is required" });

        limit = Math.Clamp(limit, 1, 50);
        var results = await searchService.SearchPostsAsync(q, limit);
        return Ok(results);
    }

    /// <summary>
    /// Search both users and posts in a single request
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(SearchResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAll([FromQuery] string q, [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Query parameter 'q' is required" });

        limit = Math.Clamp(limit, 1, 20);

        var usersTask = searchService.SearchUsersAsync(q, limit);
        var postsTask = searchService.SearchPostsAsync(q, limit);

        await Task.WhenAll(usersTask, postsTask);

        return Ok(new SearchResultDto(usersTask.Result, postsTask.Result));
    }
}

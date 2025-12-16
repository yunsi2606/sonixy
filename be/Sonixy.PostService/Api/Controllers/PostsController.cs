using Microsoft.AspNetCore.Mvc;
using Sonixy.PostService.Application.DTOs;
using Sonixy.PostService.Application.Services;

namespace Sonixy.PostService.Api.Controllers;

/// <summary>
/// Post creation and feed management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PostsController(IPostService postService) : ControllerBase
{
    /// <summary>
    /// Creates a new post
    /// </summary>
    /// <remarks>
    /// Creates a text-only post with configurable visibility.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/posts
    ///     {
    ///       "content": "Just shipped our new microservice architecture with cursor pagination!",
    ///       "visibility": "public"
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto">Post creation payload</param>
    /// <returns>The newly created post</returns>
    /// <response code="201">Post created successfully</response>
    /// <response code="400">Invalid request - content too long or empty</response>
    [HttpPost]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
    {
        // TODO: Extract userId from JWT token
        var userId = "507f1f77bcf86cd799439011"; // Placeholder
        
        var post = await postService.CreatePostAsync(dto, userId);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    /// <summary>
    /// Retrieves a single post by ID
    /// </summary>
    /// <param name="id">Post ID</param>
    /// <returns>Post details</returns>
    /// <response code="200">Post found</response>
    /// <response code="404">Post not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPost(string id)
    {
        var post = await postService.GetPostByIdAsync(id);
        
        if (post is null)
            return NotFound(new { error = "Post not found" });

        return Ok(post);
    }

    /// <summary>
    /// Retrieves the global public feed with cursor pagination
    /// </summary>
    /// <remarks>
    /// Returns public posts ordered by creation time (newest first).
    /// 
    /// **Cursor Pagination:**
    /// - First request: omit `cursor` parameter
    /// - Subsequent requests: use `nextCursor` from previous response
    /// - Page remains stable even with new posts
    /// 
    /// Example:
    /// 
    ///     GET /api/posts/feed?pageSize=20
    ///     GET /api/posts/feed?cursor=NTA3ZjFmNzdiY2Y4NmNkNzk5NDM5MDExOjYzODcwNzU5ODAwMA&amp;pageSize=20
    /// 
    /// </remarks>
    /// <param name="cursor">Pagination cursor from previous response</param>
    /// <param name="pageSize">Number of items per page (max 100)</param>
    /// <returns>Paginated list of posts</returns>
    /// <response code="200">Feed retrieved successfully</response>
    [HttpGet("feed")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeed([FromQuery] string? cursor, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, 100); // Cap at 100
        var feed = await postService.GetFeedAsync(cursor, pageSize);
        return Ok(feed);
    }

    /// <summary>
    /// Retrieves posts by a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cursor">Pagination cursor</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>User's posts</returns>
    /// <response code="200">Posts retrieved successfully</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPosts(string userId, [FromQuery] string? cursor, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, 100);
        var posts = await postService.GetUserPostsAsync(userId, cursor, pageSize);
        return Ok(posts);
    }
}

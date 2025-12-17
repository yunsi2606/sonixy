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
    // TODO: Replace with real user ID from JWT context when available middleware is ready
    private const string TestUserId = "507f1f77bcf86cd799439011"; 

    /// <summary>
    /// Generates a presigned URL for direct file upload to MinIO
    /// </summary>
    [HttpPost("presigned-url")]
    [ProducesResponseType(typeof(PresignedUrlResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GeneratePresignedUrl([FromBody] PresignedUrlRequestDto request)
    {
        var (uploadUrl, objectKey, publicUrl) = await postService.GeneratePresignedUrlAsync(request.FileName, request.ContentType);
        return Ok(new PresignedUrlResponseDto(uploadUrl, objectKey, publicUrl));
    }

    /// <summary>
    /// Creates a new post with media references
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostWithMediaDto dto)
    {
        var post = await postService.CreatePostAsync(dto, TestUserId);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    /// <summary>
    /// Retrieves a single post by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPost(string id)
    {
        var post = await postService.GetPostByIdAsync(id, TestUserId);
        
        if (post is null)
            return NotFound(new { error = "Post not found" });

        return Ok(post);
    }

    /// <summary>
    /// Retrieves the global public feed with cursor pagination
    /// </summary>
    [HttpGet("feed")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeed([FromQuery] string? cursor, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, 100); // Cap at 100
        var feed = await postService.GetFeedAsync(cursor, TestUserId, pageSize);
        return Ok(feed);
    }

    /// <summary>
    /// Retrieves posts by a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPosts(string userId, [FromQuery] string? cursor, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, 100);
        var posts = await postService.GetUserPostsAsync(userId, cursor, TestUserId, pageSize);
        return Ok(posts);
    }

    /// <summary>
    /// Toggles like on a post
    /// </summary>
    [HttpPost("{id}/like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleLike(string id)
    {
        var success = await postService.ToggleLikeAsync(id, TestUserId);
        if (!success) return NotFound(new { error = "Post not found or invalid ID" });
        return Ok(new { success = true });
    }
}

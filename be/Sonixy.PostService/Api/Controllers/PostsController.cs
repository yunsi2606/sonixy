using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
    /// Generates a presigned URL for direct file upload to MinIO
    /// </summary>
    [HttpPost("presigned-url")]
    [Authorize]
    [ProducesResponseType(typeof(PresignedUrlResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GeneratePresignedUrl([FromBody] PresignedUrlRequestDto request)
    {
        var (uploadUrl, objectKey, publicUrl) = await postService.GeneratePresignedUrlAsync(request.FileName, request.ContentType);
        return Ok(new PresignedUrlResponseDto(uploadUrl, objectKey));
    }

    /// <summary>
    /// Creates a new post with media references
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostWithMediaDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var post = await postService.CreatePostAsync(dto, userId);
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var post = await postService.GetPostByIdAsync(id, userId);
        
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        pageSize = Math.Min(pageSize, 100); // Cap at 100
        var feed = await postService.GetFeedAsync(cursor, userId, pageSize);
        return Ok(feed);
    }

    /// <summary>
    /// Retrieves posts by a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPosts(string userId, [FromQuery] string? cursor, [FromQuery] int pageSize = 20)
    {
        // For "My Posts", user might pass "me" or their ID
        if (userId.Equals("me", StringComparison.OrdinalIgnoreCase))
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        pageSize = Math.Min(pageSize, 100);
        var posts = await postService.GetUserPostsAsync(userId, cursor, currentUserId, pageSize);
        return Ok(posts);
    }

    /// <summary>
    /// Toggles like on a post
    /// </summary>
    [HttpPost("{id}/like")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleLike(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var success = await postService.ToggleLikeAsync(id, userId);
        if (!success) return NotFound(new { error = "Post not found or invalid ID" });
        return Ok(new { success = true });
    }

    /// <summary>
    /// Retrieves a batch of posts by IDs
    /// </summary>
    [HttpPost("batch")]
    [ProducesResponseType(typeof(List<PostDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostsByIds([FromBody] List<string> ids)
    {
        // currentUserId is optional, for "IsLiked" status
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var posts = await postService.GetPostsByIdsAsync(ids, userId);
        return Ok(posts);
    }
}

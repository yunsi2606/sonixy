using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonixy.PostService.Application.DTOs;
using Sonixy.PostService.Application.Interfaces;
using Sonixy.Shared.Pagination;
using System.Security.Claims;

namespace Sonixy.PostService.Api.Controllers;

[ApiController]
[Route("api")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    [HttpGet("posts/{postId}/comments")]
    public async Task<ActionResult<CursorPage<CommentDto>>> GetComments(string postId, [FromQuery] string? cursor, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await commentService.GetCommentsByPostIdAsync(postId, cursor, pageSize, userId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("posts/{postId}/comments")]
    [Authorize]
    public async Task<ActionResult<CommentDto>> CreateComment(string postId, [FromBody] CreateCommentDto dto, CancellationToken cancellationToken = default)
    {
        if (postId != dto.PostId) return BadRequest("Post ID mismatch");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try 
        {
            var comment = await commentService.CreateCommentAsync(userId, dto, cancellationToken);
            return Ok(comment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("comments/{commentId}")]
    [Authorize]
    public async Task<ActionResult> DeleteComment(string commentId, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var success = await commentService.DeleteCommentAsync(commentId, userId, cancellationToken);
        if (!success) return NotFound();

        return NoContent();
    }
}

using Sonixy.PostService.Application.DTOs;
using Sonixy.Shared.Pagination;

namespace Sonixy.PostService.Application.Interfaces;

public interface ICommentService
{
    Task<CommentDto?> CreateCommentAsync(string userId, CreateCommentDto dto, CancellationToken cancellationToken = default);
    Task<CursorPage<CommentDto>> GetCommentsByPostIdAsync(string postId, string? cursor, int pageSize = 20, string? currentUserId = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteCommentAsync(string commentId, string userId, CancellationToken cancellationToken = default);
}

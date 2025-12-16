using Sonixy.PostService.Application.DTOs;
using Sonixy.Shared.Pagination;

namespace Sonixy.PostService.Application.Services;

public interface IPostService
{
    Task<PostDto> CreatePostAsync(CreatePostDto dto, string authorId, CancellationToken cancellationToken = default);
    Task<PostDto?> GetPostByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<CursorPage<PostDto>> GetFeedAsync(string? cursor, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<CursorPage<PostDto>> GetUserPostsAsync(string userId, string? cursor, int pageSize = 20, CancellationToken cancellationToken = default);
}

using Sonixy.PostService.Application.DTOs;
using Sonixy.Shared.Pagination;

namespace Sonixy.PostService.Application.Services;

public interface IPostService
{
    Task<(string UploadUrl, string ObjectKey, string PublicUrl)> GeneratePresignedUrlAsync(string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<PostDto> CreatePostAsync(CreatePostWithMediaDto dto, string authorId, CancellationToken cancellationToken = default);
    Task<PostDto?> GetPostByIdAsync(string id, string? currentUserId = null, CancellationToken cancellationToken = default);
    Task<CursorPage<PostDto>> GetFeedAsync(string? cursor, string? currentUserId = null, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<CursorPage<PostDto>> GetUserPostsAsync(string userId, string? cursor, string? currentUserId = null, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<bool> ToggleLikeAsync(string postId, string userId, CancellationToken cancellationToken = default);
}

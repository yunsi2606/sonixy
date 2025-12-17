using MongoDB.Bson;
using Sonixy.PostService.Infrastructure.Storage;
using Microsoft.Extensions.Options;
using Sonixy.PostService.Application.DTOs;
using Sonixy.PostService.Domain.Entities;
using Sonixy.PostService.Domain.Repositories;
using Sonixy.PostService.Application.Interfaces;
using Sonixy.Shared.Pagination;

namespace Sonixy.PostService.Application.Services;

public class PostService(
    IPostRepository postRepository,
    IMediaStorage mediaStorage,
    IOptions<MinioOptions> minioOptions) : IPostService
{
    private readonly MinioOptions _minioOptions = minioOptions.Value;
    
    public async Task<(string UploadUrl, string ObjectKey, string PublicUrl)> GeneratePresignedUrlAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        return await mediaStorage.GeneratePresignedUploadUrlAsync(fileName, contentType, cancellationToken);
    }

    public async Task<PostDto> CreatePostAsync(CreatePostWithMediaDto dto, string authorId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(authorId, out var authorObjectId))
            throw new ArgumentException("Invalid author ID");

        var mediaItems = new List<MediaItem>();

        if (dto.Media != null && dto.Media.Count > 0)
        {
            foreach (var item in dto.Media)
            {
                var host = !string.IsNullOrEmpty(_minioOptions.PublicUrl) ? _minioOptions.PublicUrl : $"{(_minioOptions.UseSSL ? "https" : "http")}://{_minioOptions.Endpoint}";
                var fullUrl = $"{host}/{_minioOptions.Bucket}/{item.ObjectKey}";
                
                mediaItems.Add(new MediaItem(item.Type, fullUrl)); 
            }
        }

        var post = new Post
        {
            AuthorId = authorObjectId,
            Content = dto.Content,
            Visibility = dto.Visibility,
            LikeCount = 0,
            LikedBy = [],
            Media = mediaItems
        };

        await postRepository.AddAsync(post, cancellationToken);

        return MapToDto(post, authorId);
    }

    public async Task<PostDto?> GetPostByIdAsync(string id, string? currentUserId = null, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var post = await postRepository.GetByIdAsync(objectId, cancellationToken);
        return post is not null ? MapToDto(post, currentUserId) : null;
    }

    public async Task<CursorPage<PostDto>> GetFeedAsync(string? cursor, string? currentUserId = null, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var spec = new FeedSpecification(cursor, pageSize + 1);
        var posts = (await postRepository.FindAsync(spec, cancellationToken)).ToList();

        var hasMore = posts.Count > pageSize;
        var itemsToReturn = hasMore ? posts.Take(pageSize) : posts;

        var nextCursor = hasMore
            ? CursorHelper.EncodeCursor(posts[pageSize - 1].Id, posts[pageSize - 1].CreatedAt)
            : null;

        return new CursorPage<PostDto>(
            itemsToReturn.Select(p => MapToDto(p, currentUserId)),
            nextCursor,
            hasMore
        );
    }

    public async Task<CursorPage<PostDto>> GetUserPostsAsync(string userId, string? cursor, string? currentUserId = null, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId))
            return new CursorPage<PostDto>([], null, false);

        var spec = new UserPostsSpecification(userObjectId, cursor, pageSize + 1);
        var posts = (await postRepository.FindAsync(spec, cancellationToken)).ToList();

        var hasMore = posts.Count > pageSize;
        var itemsToReturn = hasMore ? posts.Take(pageSize) : posts;

        var nextCursor = hasMore
            ? CursorHelper.EncodeCursor(posts[pageSize - 1].Id, posts[pageSize - 1].CreatedAt)
            : null;

        return new CursorPage<PostDto>(
            itemsToReturn.Select(p => MapToDto(p, currentUserId)),
            nextCursor,
            hasMore
        );
    }

    public async Task<bool> ToggleLikeAsync(string postId, string userId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(postId, out var postObjectId) || !ObjectId.TryParse(userId, out var userObjectId))
            return false;

        var post = await postRepository.GetByIdAsync(postObjectId, cancellationToken);
        if (post is null) return false;

        post.LikedBy ??= [];

        if (post.LikedBy.Contains(userObjectId))
        {
            post.LikedBy.Remove(userObjectId);
            post.LikeCount = Math.Max(0, post.LikeCount - 1);
        }
        else
        {
            post.LikedBy.Add(userObjectId);
            post.LikeCount++;
        }

        await postRepository.UpdateAsync(post, cancellationToken);
        return true;
    }

    private static PostDto MapToDto(Post post, string? currentUserId)
    {
        var isLiked = false;
        if (!string.IsNullOrEmpty(currentUserId) && ObjectId.TryParse(currentUserId, out var userObjectId))
        {
            isLiked = post.LikedBy?.Contains(userObjectId) ?? false;
        }

        var mediaDtos = post.Media?.Select(m => new MediaItemDto(m.Type, m.Url)).ToList() ?? [];

        return new PostDto(
            post.Id.ToString(),
            post.AuthorId.ToString(),
            post.Content,
            post.Visibility,
            post.LikeCount,
            isLiked,
            mediaDtos,
            post.CreatedAt,
            post.UpdatedAt
        );
    }
}

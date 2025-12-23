using MongoDB.Bson;
using Microsoft.Extensions.Options;
using Sonixy.PostService.Application.DTOs;
using Sonixy.PostService.Domain.Entities;
using Sonixy.PostService.Domain.Repositories;
using Sonixy.Shared.Interfaces;
using Sonixy.Shared.Configuration;
using Sonixy.Shared.Pagination;
using Sonixy.PostService.Application.Interfaces;

using MassTransit;
using Sonixy.Shared.Events;

namespace Sonixy.PostService.Application.Services;

public class PostService(
    IPostRepository postRepository,
    IMediaStorage mediaStorage,
    IUserClient userClient,
    IPublishEndpoint publishEndpoint,
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
               mediaItems.Add(new MediaItem(item.Type, item.ObjectKey)); 
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

        // Publish Event for Feed Service (Fan-out)
        var imageUrls = mediaItems.Select(m => $"{_minioOptions.PublicUrl}/{_minioOptions.Bucket}/{m.ObjectKey}").ToList();
        
        await publishEndpoint.Publish(new PostCreatedEvent(
            post.Id.ToString(),
            post.AuthorId.ToString(),
            post.Content,
            imageUrls,
            post.CreatedAt
        ), cancellationToken);

        // Fetch author details for the single created post
        var author = await userClient.GetUserAsync(authorId, cancellationToken);
        return MapToDto(post, authorId, author);
    }

    public async Task<PostDto?> GetPostByIdAsync(string id, string? currentUserId = null, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var post = await postRepository.GetByIdAsync(objectId, cancellationToken);
        if (post is null) return null;

        var author = await userClient.GetUserAsync(post.AuthorId.ToString(), cancellationToken);
        return MapToDto(post, currentUserId, author);
    }

    public async Task<CursorPage<PostDto>> GetFeedAsync(string? cursor, string? currentUserId = null, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var spec = new FeedSpecification(cursor, pageSize + 1);
        var posts = (await postRepository.FindAsync(spec, cancellationToken)).ToList();

        var hasMore = posts.Count > pageSize;
        var itemsToReturn = hasMore ? posts.Take(pageSize).ToList() : posts;

        var dtos = await EnrichPostsAsync(itemsToReturn, currentUserId, cancellationToken);

        var nextCursor = hasMore
            ? CursorHelper.EncodeCursor(posts[pageSize - 1].Id, posts[pageSize - 1].CreatedAt)
            : null;

        return new CursorPage<PostDto>(dtos, nextCursor, hasMore);
    }

    public async Task<CursorPage<PostDto>> GetUserPostsAsync(string userId, string? cursor, string? currentUserId = null, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        ObjectId userObjectId;
        if (!ObjectId.TryParse(userId, out userObjectId))
        {
            // Try resolving via username
            var resolvedId = await userClient.GetUserIdByUsernameAsync(userId, cancellationToken);
            if (string.IsNullOrEmpty(resolvedId) || !ObjectId.TryParse(resolvedId, out userObjectId))
            {
                // User not found
                 return new CursorPage<PostDto>([], null, false);
            }
        }

        var spec = new UserPostsSpecification(userObjectId, cursor, pageSize + 1);
        var posts = (await postRepository.FindAsync(spec, cancellationToken)).ToList();

        var hasMore = posts.Count > pageSize;
        var itemsToReturn = hasMore ? posts.Take(pageSize).ToList() : posts;

        var dtos = await EnrichPostsAsync(itemsToReturn, currentUserId, cancellationToken);

        var nextCursor = hasMore
            ? CursorHelper.EncodeCursor(posts[pageSize - 1].Id, posts[pageSize - 1].CreatedAt)
            : null;

        return new CursorPage<PostDto>(dtos, nextCursor, hasMore);
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

    private async Task<List<PostDto>> EnrichPostsAsync(List<Post> posts, string? currentUserId, CancellationToken cancellationToken)
    {
        if (posts.Count == 0) return [];

        var authorIds = posts.Select(p => p.AuthorId.ToString()).Distinct();
        var authors = await userClient.GetUsersBatchAsync(authorIds, cancellationToken);
        var authorMap = authors.ToDictionary(u => u.Id, u => u);

        return posts.Select(p => 
        {
            authorMap.TryGetValue(p.AuthorId.ToString(), out var author);
            return MapToDto(p, currentUserId, author);
        }).ToList();
    }

    private PostDto MapToDto(Post post, string? currentUserId, UserDto? author)
    {
        var isLiked = false;
        if (!string.IsNullOrEmpty(currentUserId) && ObjectId.TryParse(currentUserId, out var userObjectId))
        {
            isLiked = post.LikedBy?.Contains(userObjectId) ?? false;
        }

        var mediaDtos = post.Media.Select(m =>
            new MediaItemDto(
                m.Type,
                $"{_minioOptions.PublicUrl}/{_minioOptions.Bucket}/{m.ObjectKey}"
            )
        ).ToList();

        // Default displayName/avatar if user service fails or user not found
        // "Unknown User" or maybe default to just "User"
        // For avatar, if null, we let frontend handle or set empty string
        var displayName = author?.DisplayName ?? "Unknown User"; 
        var avatarUrl = author?.AvatarUrl ?? "";
        var username = author?.Username ?? "";

        return new PostDto(
            post.Id.ToString(),
            post.AuthorId.ToString(),
            displayName,
            avatarUrl,
            username,
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

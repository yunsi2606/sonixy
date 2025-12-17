using MongoDB.Bson;
using Sonixy.PostService.Application.DTOs;
using Sonixy.PostService.Domain.Entities;
using Sonixy.PostService.Domain.Repositories;
using Sonixy.Shared.Pagination;

namespace Sonixy.PostService.Application.Services;

public class PostService(IPostRepository postRepository) : IPostService
{
    public async Task<PostDto> CreatePostAsync(CreatePostDto dto, string authorId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(authorId, out var authorObjectId))
            throw new ArgumentException("Invalid author ID");

        var post = new Post
        {
            AuthorId = authorObjectId,
            Content = dto.Content,
            Visibility = dto.Visibility,
            LikeCount = 0,
            LikedBy = []
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

        // Ensure LikedBy is initialized (prevent potential nulls if DB has old docs)
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

        return new PostDto(
            post.Id.ToString(),
            post.AuthorId.ToString(),
            post.Content,
            post.Visibility,
            post.LikeCount,
            isLiked,
            post.CreatedAt,
            post.UpdatedAt
        );
    }
}

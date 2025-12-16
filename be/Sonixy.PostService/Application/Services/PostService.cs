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
            LikeCount = 0
        };

        await postRepository.AddAsync(post, cancellationToken);

        return MapToDto(post);
    }

    public async Task<PostDto?> GetPostByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var post = await postRepository.GetByIdAsync(objectId, cancellationToken);
        return post is not null ? MapToDto(post) : null;
    }

    public async Task<CursorPage<PostDto>> GetFeedAsync(string? cursor, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var spec = new FeedSpecification(cursor, pageSize + 1);
        var posts = (await postRepository.FindAsync(spec, cancellationToken)).ToList();

        var hasMore = posts.Count > pageSize;
        var itemsToReturn = hasMore ? posts.Take(pageSize) : posts;

        var nextCursor = hasMore
            ? CursorHelper.EncodeCursor(posts[pageSize - 1].Id, posts[pageSize - 1].CreatedAt)
            : null;

        return new CursorPage<PostDto>(
            itemsToReturn.Select(MapToDto),
            nextCursor,
            hasMore
        );
    }

    public async Task<CursorPage<PostDto>> GetUserPostsAsync(string userId, string? cursor, int pageSize = 20, CancellationToken cancellationToken = default)
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
            itemsToReturn.Select(MapToDto),
            nextCursor,
            hasMore
        );
    }

    private static PostDto MapToDto(Post post) => new(
        post.Id.ToString(),
        post.AuthorId.ToString(),
        post.Content,
        post.Visibility,
        post.LikeCount,
        post.CreatedAt,
        post.UpdatedAt
    );
}

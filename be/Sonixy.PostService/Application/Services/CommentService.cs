using MongoDB.Bson;
using Sonixy.PostService.Application.DTOs;
using Sonixy.PostService.Application.Interfaces;
using Sonixy.PostService.Application.Specifications;
using Sonixy.PostService.Domain.Entities;
using Sonixy.PostService.Domain.Repositories;
using Sonixy.Shared.Pagination;

namespace Sonixy.PostService.Application.Services;

public class CommentService(ICommentRepository commentRepository) : ICommentService
{
    public async Task<CommentDto?> CreateCommentAsync(string userId, CreateCommentDto dto, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId))
            throw new ArgumentException("Invalid user ID");
        
        if (!ObjectId.TryParse(dto.PostId, out var postObjectId))
            throw new ArgumentException("Invalid post ID");

        ObjectId? finalParentId = null;
        if (!string.IsNullOrEmpty(dto.ParentId) && ObjectId.TryParse(dto.ParentId, out var parentId))
        {
            var parent = await commentRepository.GetByIdAsync(parentId, cancellationToken);
            if (parent != null)
            {
                // Strict Flattening Rule:
                // If the parent we are replying to is ITSELF a reply (ParentId != null),
                // then we adopt ITS ParentId.
                // Otherwise, the parent IS the root, so we use its Id.
                finalParentId = parent.ParentId ?? parentId;
            }
        }

        ObjectId? replyToUserId = null;
        if (!string.IsNullOrEmpty(dto.ReplyToUserId) && ObjectId.TryParse(dto.ReplyToUserId, out var rUserId))
        {
            replyToUserId = rUserId;
        }

        var comment = new Comment
        {
            PostId = postObjectId,
            AuthorId = userObjectId,
            AuthorUsername = dto.AuthorUsername,
            AuthorAvatarUrl = dto.AuthorAvatarUrl,
            Content = dto.Content,
            ParentId = finalParentId,
            ReplyToUserId = replyToUserId,
            ReplyToUsername = dto.ReplyToUsername,
            LikeCount = 0,
            LikedBy = []
        };

        await commentRepository.AddAsync(comment, cancellationToken);

        return MapToDto(comment, userId);
    }

    public async Task<CursorPage<CommentDto>> GetCommentsByPostIdAsync(string postId, string? cursor, int pageSize = 20, string? currentUserId = null, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(postId, out var postObjectId))
             return new CursorPage<CommentDto>([], null, false);

        var spec = new CommentsByPostSpecification(postObjectId, cursor, pageSize + 1);
        var comments = (await commentRepository.FindAsync(spec, cancellationToken)).ToList();

        var hasMore = comments.Count > pageSize;
        var itemsToReturn = hasMore ? comments.Take(pageSize) : comments;

        var nextCursor = hasMore
            ? itemsToReturn.Last().Id.ToString() // Simple cursor: using ID hex string
            : null;

        return new CursorPage<CommentDto>(
            itemsToReturn.Select(c => MapToDto(c, currentUserId)),
            nextCursor,
            hasMore
        );
    }

    public async Task<bool> DeleteCommentAsync(string commentId, string userId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(commentId, out var commentObjectId) || !ObjectId.TryParse(userId, out var userObjectId))
            return false;

        var comment = await commentRepository.GetByIdAsync(commentObjectId, cancellationToken);
        if (comment == null) return false;

        // Allow delete if author or perhaps admin (admin check omitted for simplicity)
        if (comment.AuthorId != userObjectId)
            return false;

        await commentRepository.DeleteAsync(commentObjectId, cancellationToken);
        return true;
    }

    private CommentDto MapToDto(Comment comment, string? currentUserId)
    {
        // Check if liked by current user logic here
        var isLiked = false;
        if (!string.IsNullOrEmpty(currentUserId) && ObjectId.TryParse(currentUserId, out var userObjectId))
        {
            isLiked = comment.LikedBy?.Contains(userObjectId) ?? false;
        }

        return new CommentDto(
            comment.Id.ToString(),
            comment.PostId.ToString(),
            comment.Content,
            new UserSimpleDto(
                comment.AuthorId.ToString(),
                comment.AuthorUsername,
                comment.AuthorAvatarUrl
            ),
            comment.CreatedAt.ToString("O"), // ISO 8601
            comment.ParentId?.ToString(),
            comment.ReplyToUserId.HasValue 
                ? new ReplyToDto(comment.ReplyToUserId.Value.ToString(), comment.ReplyToUsername ?? "")
                : null,
            comment.LikeCount,
            isLiked
        );
    }
}

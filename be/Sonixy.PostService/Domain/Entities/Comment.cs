using MongoDB.Bson;
using Sonixy.Shared.Common;

namespace Sonixy.PostService.Domain.Entities;

public class Comment : Entity
{
    public required ObjectId PostId { get; set; }
    public required ObjectId AuthorId { get; set; }
    public required string AuthorUsername { get; set; }
    public required string AuthorAvatarUrl { get; set; }
    public required string Content { get; set; }
    public ObjectId? ParentId { get; set; } // Null if root

    // Reply Metadata - Optional
    public ObjectId? ReplyToUserId { get; set; }
    public string? ReplyToUsername { get; set; }

    public int LikeCount { get; set; }
    public List<ObjectId> LikedBy { get; set; } = [];
}

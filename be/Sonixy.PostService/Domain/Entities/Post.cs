using MongoDB.Bson;
using Sonixy.Shared.Common;

namespace Sonixy.PostService.Domain.Entities;

public class Post : Entity
{
    public required ObjectId AuthorId { get; set; }
    public required string Content { get; set; }
    public string Visibility { get; set; } = "public"; // "public" | "followers"
    public int LikeCount { get; set; } // Denormalized for performance
}

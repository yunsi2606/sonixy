using MongoDB.Bson;
using Sonixy.Shared.Common;

namespace Sonixy.SocialGraphService.Domain.Entities;

public class Follow : Entity
{
    public required ObjectId FollowerId { get; set; }  // User who follows
    public required ObjectId FollowingId { get; set; } // User being followed
}

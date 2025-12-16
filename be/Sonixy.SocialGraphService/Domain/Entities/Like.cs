using MongoDB.Bson;
using Sonixy.Shared.Common;

namespace Sonixy.SocialGraphService.Domain.Entities;

public class Like : Entity
{
    public required ObjectId UserId { get; set; }
    public required ObjectId PostId { get; set; }
}

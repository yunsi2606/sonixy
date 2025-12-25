using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sonixy.NotificationService.Domain.Entities;

public class Notification
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    public string RecipientId { get; set; } = null!;
    public string ActorId { get; set; } = null!;
    public string ActorName { get; set; } = null!;
    public string ActorAvatar { get; set; } = null!;
    
    public string EntityId { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public string Action { get; set; } = null!; // Like, Comment, Reply
    
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

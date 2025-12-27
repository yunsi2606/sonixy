using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sonixy.Shared.Common;

namespace Sonixy.ChatService.Domain.Entities;

public class ChatParticipant : Entity
{
    public string UserId { get; set; } = string.Empty;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string ConversationId { get; set; } = string.Empty;
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string? LastReadMessageId { get; set; }
}

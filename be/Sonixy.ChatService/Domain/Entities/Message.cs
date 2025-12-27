using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sonixy.ChatService.Domain.Enums;
using Sonixy.Shared.Common;

namespace Sonixy.ChatService.Domain.Entities;

public class Message : Entity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ConversationId { get; set; } = string.Empty;
    
    public string SenderId { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public MessageType Type { get; set; } = MessageType.Text;
    
    // Additional metadata (file url, etc.) could be added later
}

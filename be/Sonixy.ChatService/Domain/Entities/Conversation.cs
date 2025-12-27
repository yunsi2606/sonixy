using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sonixy.ChatService.Domain.Enums;
using Sonixy.Shared.Common;

namespace Sonixy.ChatService.Domain.Entities;

public class Conversation : Entity
{
    public ConversationType Type { get; set; }
    
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
    
    // Denormalized fields for list view performance
    public string? LastMessageContent { get; set; }
    public string? LastMessageSenderId { get; set; }
    public MessageType LastMessageType { get; set; }
}

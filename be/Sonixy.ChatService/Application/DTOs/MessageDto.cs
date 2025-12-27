using Sonixy.ChatService.Domain.Enums;

namespace Sonixy.ChatService.Application.DTOs;

public class MessageDto
{
    public string Id { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Optional: IsReadByMe? (Calculated based on ChatParticipant.LastReadMessageId)
}

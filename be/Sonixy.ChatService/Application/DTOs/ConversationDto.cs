using Sonixy.ChatService.Domain.Enums;

namespace Sonixy.ChatService.Application.DTOs;

public class ConversationDto
{
    public string Id { get; set; } = string.Empty;
    public ConversationType Type { get; set; }
    public DateTime LastMessageAt { get; set; }
    public string? LastMessageContent { get; set; }
    public string? LastMessageSenderId { get; set; }
    public MessageType LastMessageType { get; set; }
    
    // For UI:
    public List<ParticipantDto> Participants { get; set; } = new();
    
    // Unread count? usually calculated
    public int UnreadCount { get; set; }
}

public class ParticipantDto
{
    public string UserId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
}

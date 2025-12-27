using Sonixy.ChatService.Domain.Enums;

namespace Sonixy.ChatService.Application.DTOs;

public class SendMessageDto
{
    public string ConversationId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Text;
}

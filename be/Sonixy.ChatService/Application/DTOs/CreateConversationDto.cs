using Sonixy.ChatService.Domain.Enums;

namespace Sonixy.ChatService.Application.DTOs;

public class CreateConversationDto
{
    public ConversationType Type { get; set; } = ConversationType.Private;
    public List<string> ParticipantIds { get; set; } = new(); 
    // Usually excludes self, we add self automatically.
}

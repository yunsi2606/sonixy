using Sonixy.ChatService.Application.DTOs;

namespace Sonixy.ChatService.Application.Interfaces;

public interface IChatService
{
    Task<ConversationDto> CreateConversationAsync(string creatorId, CreateConversationDto dto);
    Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(string userId);
    Task<MessageDto> SendMessageAsync(string senderId, SendMessageDto dto);
    Task<IEnumerable<MessageDto>> GetMessagesAsync(string userId, string conversationId, string? beforeId, int limit);
    Task MarkAsReadAsync(string userId, string conversationId, string messageId);
}

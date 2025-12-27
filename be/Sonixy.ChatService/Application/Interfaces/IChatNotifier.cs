using Sonixy.ChatService.Domain.Entities;

namespace Sonixy.ChatService.Application.Interfaces;

public interface IChatNotifier
{
    Task NewMessageAsync(Message message, Conversation conversation);
    Task TypingStartedAsync(string conversationId, string userId);
    Task TypingStoppedAsync(string conversationId, string userId);
}

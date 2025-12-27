using Microsoft.AspNetCore.SignalR;
using Sonixy.ChatService.Application.Interfaces;
using Sonixy.ChatService.Domain.Entities;
using Sonixy.ChatService.Infrastructure.Hubs;
using Sonixy.Shared.Common;
using Sonixy.ChatService.Domain.Specifications;

namespace Sonixy.ChatService.Infrastructure.Services;

public class SignalRNotifier(IHubContext<ChatHub> hubContext, IRepository<ChatParticipant> participantRepo)
    : IChatNotifier
{
    public async Task NewMessageAsync(Message message, Conversation conversation)
    {
        var participants = await participantRepo.FindAsync(new ChatParticipantsByConversationSpec(message.ConversationId));

        foreach (var p in participants)
        {
             await hubContext.Clients.Group($"User_{p.UserId}").SendAsync("ReceiveMessage", message);
             await hubContext.Clients.Group($"User_{p.UserId}").SendAsync("UpdateConversation", conversation);
        }
    }

    public async Task TypingStartedAsync(string conversationId, string userId)
    {
        await Task.CompletedTask;
    }

    public async Task TypingStoppedAsync(string conversationId, string userId)
    {
        await Task.CompletedTask;
    }
}

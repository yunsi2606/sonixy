using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Sonixy.ChatService.Domain.Interfaces;
using System.Security.Claims;
using Sonixy.ChatService.Domain.Entities;
using Sonixy.Shared.Common;
using Sonixy.ChatService.Domain.Specifications;

namespace Sonixy.ChatService.Infrastructure.Hubs;

[Authorize]
public class ChatHub(IPresenceTracker presenceTracker, IRepository<ChatParticipant> participantRepo)
    : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            await presenceTracker.UserConnected(userId, Context.ConnectionId);
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await presenceTracker.UserDisconnected(userId, Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task TypingStarted(string conversationId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return;

        var participants = await participantRepo.FindAsync(new ChatParticipantsByConversationSpec(conversationId));
        
        // Validation: Verify caller is a participant
        if (!participants.Any(p => p.UserId == userId))
        {
             // Caller is not a participant, do nothing or throw
             return;
        }

        foreach (var p in participants)
        {
            if (p.UserId == userId) continue;
            await Clients.Group($"User_{p.UserId}").SendAsync("UserTyping", conversationId, userId);
        }
    }

    public async Task TypingStopped(string conversationId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return;

        var participants = await participantRepo.FindAsync(new ChatParticipantsByConversationSpec(conversationId));
        
        // Validation: Verify caller is a participant
        if (!participants.Any(p => p.UserId == userId)) return;

        foreach (var p in participants)
        {
            if (p.UserId == userId) continue;
            await Clients.Group($"User_{p.UserId}").SendAsync("UserStoppedTyping", conversationId, userId);
        }
    }
}

using MongoDB.Bson;
using Sonixy.ChatService.Application.DTOs;
using Sonixy.ChatService.Application.Interfaces;
using Sonixy.ChatService.Domain.Entities;
using Sonixy.ChatService.Domain.Enums;
using Sonixy.ChatService.Domain.Interfaces;
using Sonixy.ChatService.Domain.Specifications;
using Sonixy.Shared.Common;

namespace Sonixy.ChatService.Application.Services;

public class ChatService(
    IChatRepository chatRepo,
    IRepository<Message> messageRepo,
    IRepository<ChatParticipant> participantRepo,
    IChatNotifier notifier,
    ISocialGraphClient socialGraphClient)
    : IChatService
{
    public async Task<ConversationDto> CreateConversationAsync(string creatorId, CreateConversationDto dto)
    {
        if (dto.ParticipantIds == null || !dto.ParticipantIds.Any())
            throw new ArgumentException("Participants required");
            
        var distinctParticipants = dto.ParticipantIds.Distinct().Where(id => id != creatorId).ToList();

        if (dto.Type == ConversationType.Private)
        {
            if (distinctParticipants.Count != 1)
                throw new ArgumentException("Private chat must have exactly one other participant");

            var targetUserId = distinctParticipants.First();
            var isMutual = await socialGraphClient.IsMutualFollowAsync(targetUserId);
            if (!isMutual)
                throw new InvalidOperationException("You can only chat with mutual follows");
        }
        else if (dto.Type == ConversationType.Group)
        {
             if (distinctParticipants.Count < 1)
                throw new ArgumentException("Group chat must have at least one other participant");
        }

        var allParticipants = new HashSet<string>(distinctParticipants) { creatorId };

        var conversation = new Conversation
        {
            Type = dto.Type,
            LastMessageAt = DateTime.UtcNow
        };
        await chatRepo.AddAsync(conversation);
        var convId = conversation.Id.ToString();

        var participants = allParticipants.Select(uid => new ChatParticipant
        {
            UserId = uid,
            ConversationId = convId,
            JoinedAt = DateTime.UtcNow,
            LastReadMessageId = null
        }).ToList();

        foreach (var p in participants)
        {
            await participantRepo.AddAsync(p);
        }

        var result = MapToConversationDto(conversation);
        result.Participants = participants.Select(MapToParticipantDto).ToList();
        return result;
    }

    public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(string userId)
    {
        var participantSpec = new ChatParticipantsByUserIdSpec(userId);
        var userParticipations = await participantRepo.FindAsync(participantSpec);
        
        if (!userParticipations.Any()) return new List<ConversationDto>();

        var conversationIds = userParticipations.Select(x => x.ConversationId).ToList();
        var convSpec = new ConversationsByIdsSpec(conversationIds);
        var conversations = await chatRepo.FindAsync(convSpec);
        
        return conversations.Select(MapToConversationDto).ToList();
    }

    public async Task<MessageDto> SendMessageAsync(string senderId, SendMessageDto dto)
    {
        // 1. Validate membership
        var memberSpec = new ChatParticipantByUserAndConversationSpec(senderId, dto.ConversationId);
        var isMember = await participantRepo.FindAsync(memberSpec);
        if (!isMember.Any())
            throw new UnauthorizedAccessException("You are not a participant of this conversation");

        var message = new Message
        {
            ConversationId = dto.ConversationId,
            SenderId = senderId,
            Content = dto.Content,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };
        
        await messageRepo.AddAsync(message);
        
        var conversation = await chatRepo.GetByIdAsync(ObjectId.Parse(dto.ConversationId));
        if (conversation != null)
        {
            conversation.LastMessageAt = message.CreatedAt;
            conversation.LastMessageContent = message.Content;
            conversation.LastMessageSenderId = senderId;
            conversation.LastMessageType = message.Type;
            conversation.UpdatedAt = DateTime.UtcNow;
            
            await chatRepo.UpdateAsync(conversation);
            await notifier.NewMessageAsync(message, conversation);
        }

        return MapToMessageDto(message);
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesAsync(string userId, string conversationId, string? beforeId, int limit)
    {
        var memberSpec = new ChatParticipantByUserAndConversationSpec(userId, conversationId);
        var memberships = await participantRepo.FindAsync(memberSpec);
        if (!memberships.Any()) return new List<MessageDto>(); 

        var spec = new MessagesByConversationSpec(conversationId, beforeId, limit);
        var messages = await messageRepo.FindAsync(spec);
        
        return messages.Select(MapToMessageDto).ToList();
    }

    public async Task MarkAsReadAsync(string userId, string conversationId, string messageId)
    {
        var spec = new ChatParticipantByUserAndConversationSpec(userId, conversationId);
        var participants = await participantRepo.FindAsync(spec);
        var participant = participants.FirstOrDefault();
        
        if (participant != null)
        {
             if (participant.LastReadMessageId == null || ObjectId.Parse(messageId) > ObjectId.Parse(participant.LastReadMessageId))
             {
                 participant.LastReadMessageId = messageId;
                 await participantRepo.UpdateAsync(participant);
             }
        }
    }

    // Manual Mapping Helpers
    private static ConversationDto MapToConversationDto(Conversation c)
    {
        return new ConversationDto
        {
            Id = c.Id.ToString(),
            Type = c.Type,
            LastMessageAt = c.LastMessageAt,
            LastMessageContent = c.LastMessageContent,
            LastMessageSenderId = c.LastMessageSenderId,
            LastMessageType = c.LastMessageType,
            UnreadCount = 0 // To be enriched
        };
    }

    private static ParticipantDto MapToParticipantDto(ChatParticipant p)
    {
        return new ParticipantDto
        {
            UserId = p.UserId,
            JoinedAt = p.JoinedAt
        };
    }

    private static MessageDto MapToMessageDto(Message m)
    {
         return new MessageDto
         {
             Id = m.Id.ToString(),
             ConversationId = m.ConversationId,
             SenderId = m.SenderId,
             Content = m.Content,
             Type = m.Type,
             CreatedAt = m.CreatedAt
         };
    }
}

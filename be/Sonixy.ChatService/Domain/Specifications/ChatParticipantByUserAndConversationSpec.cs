using Sonixy.ChatService.Domain.Entities;
using Sonixy.Shared.Specifications;

namespace Sonixy.ChatService.Domain.Specifications;

public class ChatParticipantByUserAndConversationSpec : BaseSpecification<ChatParticipant>
{
    public ChatParticipantByUserAndConversationSpec(string userId, string conversationId)
        : base(x => x.UserId == userId && x.ConversationId == conversationId)
    {
    }
}

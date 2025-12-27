using Sonixy.ChatService.Domain.Entities;
using Sonixy.Shared.Specifications;

namespace Sonixy.ChatService.Domain.Specifications;

public class ChatParticipantsByConversationSpec : BaseSpecification<ChatParticipant>
{
    public ChatParticipantsByConversationSpec(string conversationId)
        : base(x => x.ConversationId == conversationId)
    {
    }
}

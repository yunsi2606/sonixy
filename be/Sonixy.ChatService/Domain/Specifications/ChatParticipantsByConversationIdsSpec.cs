using Sonixy.ChatService.Domain.Entities;
using Sonixy.Shared.Common;
using Sonixy.Shared.Specifications;

namespace Sonixy.ChatService.Domain.Specifications;

public class ChatParticipantsByConversationIdsSpec : BaseSpecification<ChatParticipant>
{
    public ChatParticipantsByConversationIdsSpec(IEnumerable<string> conversationIds)
        : base(p => conversationIds.Contains(p.ConversationId))
    {
    }
}

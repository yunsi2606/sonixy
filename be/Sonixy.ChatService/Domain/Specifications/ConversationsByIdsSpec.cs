using Sonixy.ChatService.Domain.Entities;
using Sonixy.Shared.Specifications;

namespace Sonixy.ChatService.Domain.Specifications;

public class ConversationsByIdsSpec : BaseSpecification<Conversation>
{
    public ConversationsByIdsSpec(IEnumerable<string> ids)
        : base(x => ids.Contains(x.Id.ToString()))
    {
        AddOrderByDescending(x => x.LastMessageAt);
    }
}

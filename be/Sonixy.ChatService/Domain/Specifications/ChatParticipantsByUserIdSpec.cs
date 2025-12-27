using Sonixy.ChatService.Domain.Entities;
using Sonixy.Shared.Specifications;

namespace Sonixy.ChatService.Domain.Specifications;

public class ChatParticipantsByUserIdSpec : BaseSpecification<ChatParticipant>
{
    public ChatParticipantsByUserIdSpec(string userId)
        : base(x => x.UserId == userId)
    {
        AddOrderByDescending(x => x.JoinedAt);
    }
}

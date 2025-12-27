using MongoDB.Bson;
using Sonixy.ChatService.Domain.Entities;
using Sonixy.Shared.Specifications;

namespace Sonixy.ChatService.Domain.Specifications;

public class MessagesByConversationSpec : BaseSpecification<Message>
{
    public MessagesByConversationSpec(string conversationId, string? beforeId, int limit)
        : base(x => x.ConversationId == conversationId)
    {
        if (!string.IsNullOrEmpty(beforeId))
        {
            var beforeObjectId = ObjectId.Parse(beforeId);
            AddCriteria(x => x.Id < beforeObjectId);
        }

        AddOrderByDescending(x => x.CreatedAt); // Latest first
        ApplyPaging(0, limit);
    }
}

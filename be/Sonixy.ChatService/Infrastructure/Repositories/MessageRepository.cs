using MongoDB.Driver;
using Sonixy.ChatService.Domain.Entities;
using Sonixy.Shared.Common;

namespace Sonixy.ChatService.Infrastructure.Repositories;

public class MessageRepository : BaseRepository<Message>
{
    public MessageRepository(IMongoDatabase database) : base(database, "messages")
    {
        // Indexes
        var keys = Builders<Message>.IndexKeys.Ascending(x => x.ConversationId).Descending(x => x.CreatedAt);
        _collection.Indexes.CreateOne(new CreateIndexModel<Message>(keys));
    }
}

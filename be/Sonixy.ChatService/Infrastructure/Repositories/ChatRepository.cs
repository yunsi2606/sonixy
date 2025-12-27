using MongoDB.Driver;
using Sonixy.ChatService.Domain.Entities;
using Sonixy.ChatService.Domain.Interfaces;

namespace Sonixy.ChatService.Infrastructure.Repositories;

public class ChatRepository : BaseRepository<Conversation>, IChatRepository
{
    public ChatRepository(IMongoDatabase database) : base(database, "conversations")
    {
        // Indexes
         var keys = Builders<Conversation>.IndexKeys.Descending(x => x.LastMessageAt);
         _collection.Indexes.CreateOne(new CreateIndexModel<Conversation>(keys));
    }
}

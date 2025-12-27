using MongoDB.Driver;
using Sonixy.ChatService.Domain.Entities;
using Sonixy.Shared.Common;

namespace Sonixy.ChatService.Infrastructure.Repositories;

public class ChatParticipantRepository : BaseRepository<ChatParticipant>
{
    public ChatParticipantRepository(IMongoDatabase database) : base(database, "chat_participants")
    {
         // Indexes
         // Find conversations for a user
         var userKey = Builders<ChatParticipant>.IndexKeys.Ascending(x => x.UserId).Descending(x => x.JoinedAt);
         _collection.Indexes.CreateOne(new CreateIndexModel<ChatParticipant>(userKey));

         // Find participants for a conversation
         var convKey = Builders<ChatParticipant>.IndexKeys.Ascending(x => x.ConversationId);
         _collection.Indexes.CreateOne(new CreateIndexModel<ChatParticipant>(convKey));
    }
}

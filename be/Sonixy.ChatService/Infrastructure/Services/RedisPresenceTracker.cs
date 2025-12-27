using StackExchange.Redis;
using Sonixy.ChatService.Domain.Interfaces;

namespace Sonixy.ChatService.Infrastructure.Services;

public class RedisPresenceTracker(IConnectionMultiplexer redis) : IPresenceTracker
{
    private const string ONLINE_USERS_KEY = "online_users";

    public async Task UserConnected(string userId, string connectionId)
    {
        var db = redis.GetDatabase();
        await db.SetAddAsync($"user:{userId}:connections", connectionId);
        
        await db.SetAddAsync(ONLINE_USERS_KEY, userId); 
    }

    public async Task UserDisconnected(string userId, string connectionId)
    {
        var db = redis.GetDatabase();
        await db.SetRemoveAsync($"user:{userId}:connections", connectionId);
        
        // Check if any connections left
        var count = await db.SetLengthAsync($"user:{userId}:connections");
        if (count == 0) await db.SetRemoveAsync(ONLINE_USERS_KEY, userId);
    }

    public async Task<bool> IsUserOnline(string userId)
    {
        var db = redis.GetDatabase();
        return await db.SetLengthAsync($"user:{userId}:connections") > 0;
    }

    public async Task<List<string>> GetOnlineUsers(IEnumerable<string> userIds)
    {
        var db = redis.GetDatabase();
        var onlineUsers = new List<string>();
        
        // Pipeline/Batch checking
        var batch = db.CreateBatch();
        var tasks = new List<Task<long>>();
        var idsList = userIds.ToList();

        foreach (var id in idsList)
        {
            tasks.Add(batch.SetLengthAsync($"user:{id}:connections"));
        }

        batch.Execute();
        await Task.WhenAll(tasks);

        for (int i = 0; i < idsList.Count; i++)
        {
            if (tasks[i].Result > 0)
            {
                onlineUsers.Add(idsList[i]);
            }
        }

        return onlineUsers;
    }
}

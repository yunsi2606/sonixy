using MassTransit;
using StackExchange.Redis;
using Sonixy.Shared.Events;

namespace Sonixy.FeedService.Consumers;

public class PostCreatedConsumer(IConnectionMultiplexer redis, ILogger<PostCreatedConsumer> logger)
    : IConsumer<PostCreatedEvent>
{
    public async Task Consume(ConsumeContext<PostCreatedEvent> context)
    {
        var evt = context.Message;
        var db = redis.GetDatabase();

        // 1. Get Followers of evt.AuthorId
        // In V1, we don't have SocialService RPC yet. We will simulate fan-out.
        // TODO: Call SocialService (gRPC/HTTP) to get followers.
        
        var simulationFollowerIds = new List<string> { "follower-1" }; // Mock

        foreach (var followerId in simulationFollowerIds)
        {
            var key = $"sonixy:feed:timeline:{followerId}";
            var score = evt.CreatedAt.Ticks; // Time-based ranking (simple)

            // Hybrid Fan-out check: Verify if user is active (ToDo)
            
            await db.SortedSetAddAsync(key, evt.PostId, score);
        }

        logger.LogInformation("Fanned out Post {PostId} to {Count} followers", evt.PostId, simulationFollowerIds.Count);
    }
}

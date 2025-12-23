using MassTransit;
using StackExchange.Redis;
using Sonixy.Shared.Events;

namespace Sonixy.FeedService.Consumers;

public class PostCreatedConsumer : IConsumer<PostCreatedEvent>
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<PostCreatedConsumer> _logger;

    public PostCreatedConsumer(IConnectionMultiplexer redis, ILogger<PostCreatedConsumer> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PostCreatedEvent> context)
    {
        var evt = context.Message;
        var db = _redis.GetDatabase();

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

        _logger.LogInformation("Fanned out Post {PostId} to {Count} followers", evt.PostId, simulationFollowerIds.Count);
    }
}

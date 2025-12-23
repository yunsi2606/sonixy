using MassTransit;
using StackExchange.Redis;
using Sonixy.Shared.Events;

namespace Sonixy.FeedService.Consumers;

public class UserInteractionConsumer(IConnectionMultiplexer redis, ILogger<UserInteractionConsumer> logger) : IConsumer<UserInteractionEvent>
{
    private const double DecayFactor = 0.9; 

    public async Task Consume(ConsumeContext<UserInteractionEvent> context)
    {
        var evt = context.Message;
        var db = redis.GetDatabase();
        var key = $"sonixy:interest:{evt.UserId}";

        // Simple scoring based on ActionType
        double score = evt.ActionType switch
        {
            UserActionType.Like => 5.0,
            UserActionType.Comment => 8.0,
            UserActionType.Share => 10.0,
            UserActionType.View => 1.0, 
            UserActionType.Scroll => -0.5,
            _ => 0
        };

        if (score == 0) return;

        string topic = "General"; 

        // Update score with weight
        await db.SortedSetIncrementAsync(key, topic, score);
        
        logger.LogInformation("Updated Interest: User {UserId}, Score {Score}", evt.UserId, score);
    }
}

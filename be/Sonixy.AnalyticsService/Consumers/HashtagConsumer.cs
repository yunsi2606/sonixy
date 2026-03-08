using MassTransit;
using MongoDB.Driver;
using Sonixy.Shared.Events;

namespace Sonixy.AnalyticsService.Consumers;

/// <summary>
/// Consumes PostCreatedEvent and aggregates hashtag counts for trending feature
/// </summary>
public class HashtagConsumer : IConsumer<PostCreatedEvent>
{
    private readonly IMongoCollection<HashtagStat> _collection;
    private readonly ILogger<HashtagConsumer> _logger;

    public HashtagConsumer(IMongoDatabase database, ILogger<HashtagConsumer> logger)
    {
        _collection = database.GetCollection<HashtagStat>("HashtagStats");
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PostCreatedEvent> context)
    {
        var evt = context.Message;
        
        if (evt.Hashtags == null || evt.Hashtags.Count == 0)
        {
            _logger.LogDebug("Post {PostId} has no hashtags, skipping", evt.PostId);
            return;
        }

        foreach (var tag in evt.Hashtags)
        {
            var filter = Builders<HashtagStat>.Filter.Eq(h => h.Tag, tag);
            var update = Builders<HashtagStat>.Update
                .Inc(h => h.Count, 1)
                .Set(h => h.LastUpdated, DateTime.UtcNow)
                .SetOnInsert(h => h.Tag, tag);

            await _collection.UpdateOneAsync(
                filter, 
                update, 
                new UpdateOptions { IsUpsert = true }
            );
        }

        _logger.LogInformation("Processed {Count} hashtags from post {PostId}", 
            evt.Hashtags.Count, evt.PostId);
    }
}

/// <summary>
/// MongoDB document for hashtag statistics
/// </summary>
public class HashtagStat
{
    public object? Id { get; set; }
    public string Tag { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime LastUpdated { get; set; }
}

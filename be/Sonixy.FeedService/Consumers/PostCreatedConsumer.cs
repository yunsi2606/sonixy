using MassTransit;
using StackExchange.Redis;
using Sonixy.Shared.Events;
using Sonixy.FeedService.Services;
using System.Text.Json;

namespace Sonixy.FeedService.Consumers;

public class PostCreatedConsumer(
    IConnectionMultiplexer redis,
    ISocialClient socialClient,
    ILogger<PostCreatedConsumer> logger
) : IConsumer<PostCreatedEvent>
{
    private const int MaxTimelineSize = 500;
    private static readonly TimeSpan PostCacheTtl = TimeSpan.FromDays(7);

    public async Task Consume(ConsumeContext<PostCreatedEvent> context)
    {
        var evt = context.Message;
        var db = redis.GetDatabase();
        var score = new DateTimeOffset(evt.CreatedAt).ToUnixTimeMilliseconds();

        // Cache post metadata in Redis for fast reads later
        var postCacheKey = $"sonixy:feed:post:{evt.PostId}";
        var postCacheData = JsonSerializer.Serialize(new
        {
            id = evt.PostId,
            authorId = evt.AuthorId,
            content = evt.Content,
            imageUrls = evt.ImageUrls,
            hashtags = evt.Hashtags,
            createdAt = evt.CreatedAt
        });
        await db.StringSetAsync(postCacheKey, postCacheData, PostCacheTtl);

        // Get real followers from SocialGraphService via gRPC
        var followerIds = await socialClient.GetFollowerIdsAsync(evt.AuthorId);

        // Always include the author themselves (they should see their own post)
        var allRecipients = new HashSet<string>(followerIds) { evt.AuthorId };

        logger.LogInformation(
            "Fan-out Post {PostId} by {AuthorId} to {Count} recipients ({FollowerCount} followers + author)",
            evt.PostId, evt.AuthorId, allRecipients.Count, followerIds.Count
        );

        // 4. Fan-out: Push postId into each recipient's timeline (Redis Sorted Set)
        var batch = db.CreateBatch();
        var tasks = new List<Task>();

        foreach (var recipientId in allRecipients)
        {
            var timelineKey = $"sonixy:feed:timeline:{recipientId}";

            // Add post to timeline with time-based score
            tasks.Add(batch.SortedSetAddAsync(timelineKey, evt.PostId, score));

            // Trim timeline to prevent unbounded growth (keep only latest N posts)
            tasks.Add(batch.SortedSetRemoveRangeByRankAsync(timelineKey, 0, -(MaxTimelineSize + 1)));
        }

        batch.Execute();
        await Task.WhenAll(tasks);

        logger.LogInformation("Fan-out completed for Post {PostId}", evt.PostId);
    }
}

using MassTransit;
using StackExchange.Redis;
using Sonixy.Shared.Events;
using Sonixy.FeedService.Services;
using System.Text.Json;
using Sonixy.FeedService.Helpers;

namespace Sonixy.FeedService.Consumers;

public class UserInteractionConsumer(
    IConnectionMultiplexer redis,
    ILogger<UserInteractionConsumer> logger
) : IConsumer<UserInteractionEvent>
{
    public async Task Consume(ConsumeContext<UserInteractionEvent> context)
    {
        var evt = context.Message;

        // Only process Post-targeted interactions that affect HotScore
        if (evt.TargetType != TargetType.Post) return;
        if (evt.ActionType is not (UserActionType.Like or UserActionType.Comment or UserActionType.Share or UserActionType.Reply))
            return;

        var db = redis.GetDatabase();
        var postId = evt.TargetId;

        //  Update interaction counters in Redis (atomic increments)
        var counterKey = $"sonixy:feed:stats:{postId}";

        var field = evt.ActionType switch
        {
            UserActionType.Like => "likes",
            UserActionType.Comment => "comments",
            UserActionType.Share => "shares",
            UserActionType.Reply => "comments", // Replies count as comments for scoring
            _ => null
        };

        if (field == null) return;

        await db.HashIncrementAsync(counterKey, field, 1);
        // Set expiry on counter key (same as post cache TTL)
        await db.KeyExpireAsync(counterKey, TimeSpan.FromDays(7), ExpireWhen.HasNoExpiry);

        // Read current stats + post creation time to recalculate HotScore
        var stats = await db.HashGetAllAsync(counterKey);
        int likeCount = (int)stats.FirstOrDefault(e => e.Name == "likes").Value;
        int commentCount = (int)stats.FirstOrDefault(e => e.Name == "comments").Value;
        int shareCount = (int)stats.FirstOrDefault(e => e.Name == "shares").Value;

        // Get post creation time from cache
        var postCacheKey = $"sonixy:feed:post:{postId}";
        var cachedPost = await db.StringGetAsync(postCacheKey);

        DateTime createdAt;
        if (cachedPost.HasValue)
        {
            try
            {
                using var doc = JsonDocument.Parse(cachedPost.ToString());
                var root = doc.RootElement;
                createdAt = root.GetProperty("createdAt").GetDateTime();
            }
            catch
            {
                // Fallback: assume recent post
                createdAt = DateTime.UtcNow.AddHours(-1);
            }
        }
        else
        {
            // Post not in cache (expired or never cached), cannot recalculate accurately
            logger.LogWarning("Post {PostId} not found in cache, skipping HotScore update", postId);
            return;
        }

        // Calculate new HotScore
        var newScore = HotScoreCalculator.CalculateHotScore(
            createdAt, likeCount, commentCount, shareCount);

        // Update the score in ALL timelines that contain this post
        // We use a reverse index to know which timelines have this post
        var reverseKey = $"sonixy:feed:postline:{postId}";
        var timelineUserIds = await db.SetMembersAsync(reverseKey);

        if (timelineUserIds.Length == 0)
        {
            // Fallback: update author's timeline and the interacting user's timeline
            var authorTimeline = $"sonixy:feed:timeline:{evt.TargetUserId}";
            await db.SortedSetAddAsync(authorTimeline, postId, newScore);
            logger.LogInformation(
                "HotScore updated for Post {PostId}: {Score} (L:{Likes} C:{Comments} S:{Shares}) - author only",
                postId, newScore, likeCount, commentCount, shareCount);
            return;
        }

        var batch = db.CreateBatch();
        var tasks = new List<Task>();

        foreach (var userId in timelineUserIds)
        {
            var timelineKey = $"sonixy:feed:timeline:{userId}";
            tasks.Add(batch.SortedSetAddAsync(timelineKey, postId, newScore));
        }

        batch.Execute();
        await Task.WhenAll(tasks);

        logger.LogInformation(
            "HotScore updated for Post {PostId}: {Score} (L:{Likes} C:{Comments} S:{Shares}) across {Count} timelines",
            postId, newScore, likeCount, commentCount, shareCount, timelineUserIds.Length);

        // Also update user interest scoring (keep existing behavior)
        double interestScore = evt.ActionType switch
        {
            UserActionType.Like => 5.0,
            UserActionType.Comment => 8.0,
            UserActionType.Share => 10.0,
            UserActionType.Reply => 6.0,
            _ => 0
        };

        if (interestScore > 0)
        {
            var interestKey = $"sonixy:interest:{evt.UserId}";
            await db.SortedSetIncrementAsync(interestKey, "General", interestScore);
        }
    }
}

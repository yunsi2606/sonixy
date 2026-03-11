using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Sonixy.FeedService.Services;
using Sonixy.FeedService.DTOs;
using System.Text.Json;

namespace Sonixy.FeedService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeedController(IConnectionMultiplexer redis, IPostClient postClient) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFeed(
        [FromQuery] string? cursor = null,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        pageSize = Math.Clamp(pageSize, 1, 50);

        var db = redis.GetDatabase();
        var timelineKey = $"sonixy:feed:timeline:{userId}";

        // Determine score range for cursor-based pagination
        // Score is HotScore (base = Unix timestamp ms + interaction boost). Higher = hotter.
        double maxScore = double.PositiveInfinity;
        if (!string.IsNullOrEmpty(cursor) && double.TryParse(cursor, out var parsedCursor))
        {
            // Exclusive: fetch posts with lower score than cursor
            maxScore = parsedCursor - 0.001;
        }

        // Fetch pageSize + 1 to determine hasMore
        var entries = await db.SortedSetRangeByScoreWithScoresAsync(
            timelineKey,
            start: double.NegativeInfinity,
            stop: maxScore,
            exclude: Exclude.None,
            order: Order.Descending,
            skip: 0,
            take: pageSize + 1
        );

        var hasMore = entries.Length > pageSize;
        var itemsToProcess = hasMore ? entries.Take(pageSize).ToArray() : entries;

        if (itemsToProcess.Length == 0)
        {
            // Fallback: if user has no personalized timeline yet, return empty gracefully
            return Ok(new { items = Array.Empty<PostDto>(), nextCursor = (string?)null, hasMore = false });
        }

        var postIds = itemsToProcess.Select(e => e.Element.ToString()).ToList();

        // Try to read from Redis post cache first (fast path)
        var posts = new List<PostDto>();
        var missingIds = new List<string>();

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        foreach (var id in postIds)
        {
            var cacheKey = $"sonixy:feed:post:{id}";
            var cached = await db.StringGetAsync(cacheKey);
            if (cached.HasValue)
            {
                var dto = JsonSerializer.Deserialize<PostDto>(cached.ToString(), jsonOptions);
                // Only accept fully formed DTOs from cache. If it misses AuthorUsername, it's the incomplete cache from consumer.
                if (dto != null && !string.IsNullOrEmpty(dto.Id) && !string.IsNullOrEmpty(dto.AuthorUsername)) 
                { 
                    posts.Add(dto); 
                    continue; 
                }
            }
            missingIds.Add(id);
        }

        // Fallback: fetch missing posts from PostService via gRPC
        if (missingIds.Count > 0)
        {
            var fetched = await postClient.GetPostsByIdsAsync(missingIds);
            posts.AddRange(fetched);

            // Cache the fully formed posts back to Redis for future queries
            var batch = db.CreateBatch();
            var tasks = new List<Task>();
            foreach (var post in fetched)
            {
                var cacheKey = $"sonixy:feed:post:{post.Id}";
                var postCacheData = JsonSerializer.Serialize(post);
                tasks.Add(batch.StringSetAsync(cacheKey, postCacheData, TimeSpan.FromDays(7)));
            }
            batch.Execute();
            await Task.WhenAll(tasks);
        }

        // Re-order posts to match timeline order (sorted by HotScore descending)
        var postMap = posts.ToDictionary(p => p.Id);
        var orderedPosts = postIds
            .Where(id => postMap.ContainsKey(id))
            .Select(id => postMap[id])
            .ToList();

        // Build cursor from the last item's score
        string? nextCursor = null;
        if (hasMore && itemsToProcess.Length > 0)
        {
            var lastScore = itemsToProcess.Last().Score;
            nextCursor = lastScore.ToString("F3");
        }

        return Ok(new { items = orderedPosts, nextCursor, hasMore });
    }
}

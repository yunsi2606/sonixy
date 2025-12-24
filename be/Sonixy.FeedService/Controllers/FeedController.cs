using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Sonixy.FeedService.Services;
using Sonixy.FeedService.DTOs;

namespace Sonixy.FeedService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedController(IConnectionMultiplexer redis) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFeed([FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId)) return BadRequest("UserId required");

        var db = redis.GetDatabase();
        var key = $"sonixy:feed:timeline:{userId}";

        // Fetch Timeline (Last 50 posts)
        var postIdsRedis = await db.SortedSetRangeByRankAsync(key, 0, 49, Order.Descending);
        var postIds = postIdsRedis.Select(x => x.ToString()).ToList();

        if (postIds.Count == 0) return Ok(new List<PostDto>());

        // Fetch Post Data from Redis Cache (Materialized View)
        var orderedPosts = new List<PostDto>();
        var batch = db.CreateBatch();
        var tasks = new List<Task<RedisValue>>();

        foreach (var id in postIds)
        {
            var postKey = $"sonixy:feed:post:{id}";
             tasks.Add(batch.StringGetAsync(postKey));
        }
        batch.Execute();
        
        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            if (result.HasValue)
            {
                var post = System.Text.Json.JsonSerializer.Deserialize<PostDto>(result.ToString());
                if (post != null)
                {
                    orderedPosts.Add(post);
                }
            }
        }

        return Ok(new { items = orderedPosts, hasMore = false });
    }
}

using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Sonixy.FeedService.Services;
using Sonixy.FeedService.DTOs;

namespace Sonixy.FeedService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedController(IConnectionMultiplexer redis, IPostClient postClient) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFeed([FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId)) return BadRequest("UserId required");

        var db = redis.GetDatabase();
        var key = $"sonixy:feed:timeline:{userId}";

        // 1. Fetch Timeline (Last 50 posts)
        var postIdsRedis = await db.SortedSetRangeByRankAsync(key, 0, 49, Order.Descending);
        var postIds = postIdsRedis.Select(x => x.ToString()).ToList();

        if (postIds.Count == 0) return Ok(new List<PostDto>());

        // 2. Hydrate via Post Service
        var posts = await postClient.GetPostsByIdsAsync(postIds);

        // 3. Re-sort to match Redis order (since PostService might return unordered or different order)
        var postMap = posts.ToDictionary(p => p.Id, p => p);
        var orderedPosts = new List<PostDto>();

        foreach (var id in postIds)
        {
            if (postMap.TryGetValue(id, out var post))
            {
                orderedPosts.Add(post);
            }
        }

        return Ok(new { items = orderedPosts, hasMore = false });
    }

    [HttpGet("network/{userId}")]
    public IActionResult GetUserNetwork(string userId)
    {
        // Mock
        return Ok(new 
        { 
            FollowersCount = 0, 
            FollowingCount = 0,
            Followers = new List<object>(),
            Following = new List<object>() 
        });
    }
}

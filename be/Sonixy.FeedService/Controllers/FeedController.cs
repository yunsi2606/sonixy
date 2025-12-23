using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

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

        // 1. Fetch Timeline (Last 50 posts)
        var postIds = await db.SortedSetRangeByRankAsync(key, 0, 49, Order.Descending);

        // 2. Hydrate & Rank (ToDo in V2)
        // Currently returning just IDs
        return Ok(postIds.Select(x => x.ToString()));
    }
}

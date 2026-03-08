using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Sonixy.AnalyticsService.Consumers;
using Sonixy.Shared.Events;

namespace Sonixy.AnalyticsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMongoDatabase _database;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IPublishEndpoint publishEndpoint, IMongoDatabase database, ILogger<AnalyticsController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _database = database;
        _logger = logger;
    }

    [HttpPost("events")]
    public async Task<IActionResult> PublishEvent([FromBody] UserInteractionEvent interactionEvent)
    {
        if (string.IsNullOrEmpty(interactionEvent.UserId) || string.IsNullOrEmpty(interactionEvent.TargetId))
        {
            return BadRequest("UserId and TargetId are required.");
        }

        // Fire-and-forget
        await _publishEndpoint.Publish(interactionEvent);
        
        _logger.LogInformation("Published interaction: {Action} on {Target} by {User}", 
            interactionEvent.ActionType, interactionEvent.TargetId, interactionEvent.UserId);

        return Accepted();
    }

    /// <summary>
    /// Get trending hashtags sorted by usage count
    /// </summary>
    [HttpGet("trending/hashtags")]
    public async Task<IActionResult> GetTrendingHashtags([FromQuery] int limit = 10)
    {
        limit = Math.Clamp(limit, 1, 50);

        var collection = _database.GetCollection<HashtagStat>("HashtagStats");
        
        var hashtags = await collection
            .Find(_ => true)
            .SortByDescending(h => h.Count)
            .Limit(limit)
            .ToListAsync();

        var result = hashtags.Select(h => new { tag = h.Tag, count = h.Count });
        return Ok(result);
    }
}

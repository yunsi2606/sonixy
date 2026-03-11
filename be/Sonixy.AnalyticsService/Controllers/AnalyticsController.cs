using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Sonixy.AnalyticsService.Consumers;
using Sonixy.Shared.Events;

namespace Sonixy.AnalyticsService.Controllers;

public class AnalyticsEventRequest
{
    public string EventType { get; set; }
    public string TargetId { get; set; }
    public string TargetType { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(
    IPublishEndpoint publishEndpoint,
    IMongoDatabase database,
    ILogger<AnalyticsController> logger)
    : Controller
{
    [HttpPost("events")]
    public async Task<IActionResult> PublishEvent([FromBody] AnalyticsEventRequest request)
    {
        if (string.IsNullOrEmpty(request.TargetId) || string.IsNullOrEmpty(request.EventType))
        {
            return BadRequest("TargetId and EventType are required.");
        }

        // Extract UserId from JWT Token manually since we don't have Auth middleware
        string? userId = null;
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to parse JWT token");
            }
        }

        if (string.IsNullOrEmpty(userId))
        {
            // For now, allow anonymous tracking if necessary, or just skip user interaction logging
            // Since UserId is usually required for interaction logs, we can use a dummy UUID or return 400.
            return BadRequest("User identification failed. Missing or invalid token.");
        }

        if (!Enum.TryParse<UserActionType>(request.EventType, true, out var actionType))
        {
            return BadRequest("Invalid EventType.");
        }

        if (!Enum.TryParse<TargetType>(request.TargetType, true, out var targetType))
        {
            return BadRequest("Invalid TargetType.");
        }

        var interactionEvent = new UserInteractionEvent(
            UserId: userId,
            TargetId: request.TargetId,
            TargetType: targetType,
            ActionType: actionType,
            Timestamp: DateTime.UtcNow
        );

        // Fire-and-forget
        await publishEndpoint.Publish(interactionEvent);
        
        logger.LogInformation("Published interaction: {Action} on {Target} by {User}", 
            interactionEvent.ActionType, interactionEvent.TargetId, interactionEvent.UserId);

        return Ok(new { success = true });
    }

    /// <summary>
    /// Get trending hashtags sorted by usage count
    /// </summary>
    [HttpGet("trending/hashtags")]
    public async Task<IActionResult> GetTrendingHashtags([FromQuery] int limit = 10)
    {
        limit = Math.Clamp(limit, 1, 50);

        var collection = database.GetCollection<HashtagStat>("HashtagStats");
        
        var hashtags = await collection
            .Find(_ => true)
            .SortByDescending(h => h.Count)
            .Limit(limit)
            .ToListAsync();

        var result = hashtags.Select(h => new { tag = h.Tag, count = h.Count });
        return Ok(result);
    }
}

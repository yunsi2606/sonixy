using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sonixy.Shared.Events;

namespace Sonixy.AnalyticsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IPublishEndpoint publishEndpoint, ILogger<AnalyticsController> logger)
    {
        _publishEndpoint = publishEndpoint;
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
}

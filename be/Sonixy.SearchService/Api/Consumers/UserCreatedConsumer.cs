using MassTransit;
using Sonixy.SearchService.Application.Services;
using Sonixy.Shared.Events;

namespace Sonixy.SearchService.Api.Consumers;

/// <summary>
/// Consumes UserCreatedEvent from RabbitMQ and indexes user in Elasticsearch
/// </summary>
public class UserCreatedConsumer(ISearchService searchService, ILogger<UserCreatedConsumer> logger) 
    : IConsumer<UserCreatedEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received UserCreatedEvent for user {UserId}", evt.UserId);

        await searchService.IndexUserAsync(
            evt.UserId,
            evt.Username,
            evt.DisplayName,
            evt.AvatarUrl,
            evt.Bio,
            context.CancellationToken
        );

        logger.LogInformation("Successfully indexed user {UserId} in Elasticsearch", evt.UserId);
    }
}

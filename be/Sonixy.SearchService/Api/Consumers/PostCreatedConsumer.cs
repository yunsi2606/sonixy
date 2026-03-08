using MassTransit;
using Sonixy.SearchService.Application.Services;
using Sonixy.Shared.Events;

namespace Sonixy.SearchService.Api.Consumers;

/// <summary>
/// Consumes PostCreatedEvent from RabbitMQ and indexes post in Elasticsearch
/// </summary>
public class PostCreatedConsumer(ISearchService searchService, ILogger<PostCreatedConsumer> logger) 
    : IConsumer<PostCreatedEvent>
{
    public async Task Consume(ConsumeContext<PostCreatedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received PostCreatedEvent for post {PostId}", evt.PostId);

        await searchService.IndexPostAsync(
            evt.PostId,
            evt.AuthorId,
            evt.Content,
            evt.Hashtags,
            evt.CreatedAt,
            context.CancellationToken
        );

        logger.LogInformation("Successfully indexed post {PostId} with {HashtagCount} hashtags in Elasticsearch", 
            evt.PostId, evt.Hashtags.Count);
    }
}

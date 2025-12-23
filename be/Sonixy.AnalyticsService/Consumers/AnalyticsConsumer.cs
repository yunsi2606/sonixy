using MassTransit;
using MongoDB.Driver;
using Sonixy.Shared.Events;

namespace Sonixy.AnalyticsService.Consumers;

public class AnalyticsConsumer : IConsumer<UserInteractionEvent>
{
    private readonly IMongoCollection<UserInteractionLog> _collection;
    private readonly ILogger<AnalyticsConsumer> _logger;

    public AnalyticsConsumer(IMongoDatabase database, ILogger<AnalyticsConsumer> logger)
    {
        _collection = database.GetCollection<UserInteractionLog>("UserLogs");
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserInteractionEvent> context)
    {
        var evt = context.Message;
        var log = new UserInteractionLog
        {
            UserId = evt.UserId,
            TargetId = evt.TargetId,
            TargetType = evt.TargetType.ToString(),
            ActionType = evt.ActionType.ToString(),
            DurationMs = evt.DurationMs,
            Timestamp = evt.Timestamp == default ? DateTime.UtcNow : evt.Timestamp
        };

        await _collection.InsertOneAsync(log);
        _logger.LogInformation("Archived log for User: {UserId}, Action: {Action}", evt.UserId, evt.ActionType);
    }
}

public class UserInteractionLog
{
    public object Id { get; set; }
    public string UserId { get; set; }
    public string TargetId { get; set; }
    public string TargetType { get; set; }
    public string ActionType { get; set; }
    public int DurationMs { get; set; }
    public DateTime Timestamp { get; set; }
}

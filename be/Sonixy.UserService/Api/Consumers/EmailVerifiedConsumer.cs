using MassTransit;
using MongoDB.Driver;
using Sonixy.Shared.Events;
using Sonixy.UserService.Domain.Entities;

namespace Sonixy.UserService.Consumers;

public class EmailVerifiedConsumer(IMongoDatabase database, ILogger<EmailVerifiedConsumer> logger)
    : IConsumer<EmailVerifiedEvent>
{
    private readonly IMongoCollection<User> _collection = database.GetCollection<User>("users");

    public async Task Consume(ConsumeContext<EmailVerifiedEvent> context)
    {
        var message = context.Message;
        logger.LogInformation("Processing EmailVerifiedEvent for User: {UserId}", message.UserId);

        var filter = Builders<User>.Filter.Eq(u => u.Email, message.Email);
        var update = Builders<User>.Update.Set(u => u.IsEmailVerified, true);

        var result = await _collection.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            logger.LogWarning("User with email {Email} not found in UserService during email verification sync.", message.Email);
        }
        else
        {
            logger.LogInformation("Successfully updated IsEmailVerified for User: {UserId}", message.UserId);
        }
    }
}

using MongoDB.Driver;
using Sonixy.NotificationService.Domain.Entities;
using Sonixy.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace Sonixy.NotificationService.Infrastructure.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<List<Notification>> GetUserNotificationsAsync(string userId, int pageIndex, int pageSize);
    Task MarkAsReadAsync(string notificationId);
    Task MarkAllAsReadAsync(string userId);
    Task<long> GetUnreadCountAsync(string userId);
}

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _collection;

    public NotificationRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Notification>("notifications");
        
        // Index optimization
        var indexKeys = Builders<Notification>.IndexKeys.Ascending(n => n.RecipientId).Descending(n => n.CreatedAt);
        _collection.Indexes.CreateOne(new CreateIndexModel<Notification>(indexKeys));
    }

    public async Task AddAsync(Notification notification)
    {
        await _collection.InsertOneAsync(notification);
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(string userId, int pageIndex, int pageSize)
    {
        return await _collection.Find(n => n.RecipientId == userId)
            .SortByDescending(n => n.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        if (MongoDB.Bson.ObjectId.TryParse(notificationId, out var objectId))
        {
            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
            await _collection.UpdateOneAsync(n => n.Id == objectId, update);
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
        await _collection.UpdateManyAsync(n => n.RecipientId == userId && !n.IsRead, update);
    }

    public async Task<long> GetUnreadCountAsync(string userId)
    {
        return await _collection.CountDocumentsAsync(n => n.RecipientId == userId && !n.IsRead);
    }
}

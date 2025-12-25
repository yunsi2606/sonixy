using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.NotificationService.Domain.Entities;
using Sonixy.NotificationService.Domain.Enums;
using Sonixy.NotificationService.Domain.Repositories;
using Sonixy.Shared.Specifications;

namespace Sonixy.NotificationService.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _collection;

    public NotificationRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Notification>("notifications");

        // Create indexes
        var indexes = new[]
        {
            new CreateIndexModel<Notification>(
                Builders<Notification>.IndexKeys.Ascending(n => n.RecipientId).Descending(n => n.CreatedAt)
            ),
            new CreateIndexModel<Notification>(
                Builders<Notification>.IndexKeys.Ascending(n => n.RecipientId).Ascending(n => n.Status)
            )
        };
        _collection.Indexes.CreateManyAsync(indexes);
    }

    public async Task<Notification?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> FindAsync(ISpecification<Notification> specification, CancellationToken cancellationToken = default)
    {
        var query = _collection.Find(specification.ToFilter());

        if (specification.ToSort() is not null)
            query = query.Sort(specification.ToSort());

        if (specification.Skip.HasValue)
            query = query.Skip(specification.Skip.Value);

        if (specification.Limit.HasValue)
            query = query.Limit(specification.Limit.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<long> CountAsync(ISpecification<Notification> specification, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(specification.ToFilter(), cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default)
    {
         await _collection.ReplaceOneAsync(
            p => p.Id == entity.Id,
            entity,
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
         await _collection.DeleteOneAsync(p => p.Id == id, cancellationToken);
    }

    // Custom Methods
    public async Task<IEnumerable<Notification>> GetByRecipientIdAsync(string recipientId, int page, int pageSize)
    {
        return await _collection
            .Find(n => n.RecipientId == recipientId)
            .SortByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<long> GetUnreadCountAsync(string recipientId)
    {
        return await _collection
            .CountDocumentsAsync(n => n.RecipientId == recipientId && n.Status == NotificationStatus.Unread);
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        if(ObjectId.TryParse(notificationId, out var id))
        {
             var filter = Builders<Notification>.Filter.Eq(n => n.Id, id);
             var update = Builders<Notification>.Update.Set(n => n.Status, NotificationStatus.Read);
             await _collection.UpdateOneAsync(filter, update);
        }
    }

    public async Task MarkAllAsReadAsync(string recipientId)
    {
        var filter = Builders<Notification>.Filter.Eq(n => n.RecipientId, recipientId) & 
                     Builders<Notification>.Filter.Eq(n => n.Status, NotificationStatus.Unread);
        var update = Builders<Notification>.Update.Set(n => n.Status, NotificationStatus.Read);
        await _collection.UpdateManyAsync(filter, update);
    }
}

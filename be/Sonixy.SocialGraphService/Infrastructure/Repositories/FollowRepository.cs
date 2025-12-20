using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.Shared.Specifications;
using Sonixy.SocialGraphService.Domain.Entities;
using Sonixy.SocialGraphService.Domain.Repositories;

namespace Sonixy.SocialGraphService.Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly IMongoCollection<Follow> _collection;

    public FollowRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Follow>("follows");
        
        // Create indexes
        var indexes = new[]
        {
            new CreateIndexModel<Follow>(
                Builders<Follow>.IndexKeys
                    .Ascending(f => f.FollowerId)
                    .Ascending(f => f.FollowingId),
                new CreateIndexOptions { Unique = true }
            ),
            new CreateIndexModel<Follow>(
                Builders<Follow>.IndexKeys.Ascending(f => f.FollowerId).Descending(f => f.CreatedAt)
            ),
            new CreateIndexModel<Follow>(
                Builders<Follow>.IndexKeys.Ascending(f => f.FollowingId).Descending(f => f.CreatedAt)
            )
        };
        _collection.Indexes.CreateManyAsync(indexes);
    }

    public async Task<Follow?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(f => f.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Follow?> GetFollowAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(f => f.FollowerId == followerId && f.FollowingId == followingId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsFollowingAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(f => f.FollowerId == followerId && f.FollowingId == followingId)
            .AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Follow>> FindAsync(ISpecification<Follow> specification, CancellationToken cancellationToken = default)
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

    public async Task<long> CountAsync(ISpecification<Follow> specification, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(specification.ToFilter(), cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Follow entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Follow entity, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(
            f => f.Id == entity.Id,
            entity,
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<long> GetFollowerCountAsync(ObjectId userId, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(f => f.FollowingId == userId, cancellationToken: cancellationToken);
    }

    public async Task<long> GetFollowingCountAsync(ObjectId userId, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(f => f.FollowerId == userId, cancellationToken: cancellationToken);
    }
}

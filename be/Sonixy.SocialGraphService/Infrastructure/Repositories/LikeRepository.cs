using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.Shared.Specifications;
using Sonixy.SocialGraphService.Domain.Entities;
using Sonixy.SocialGraphService.Domain.Repositories;

namespace Sonixy.SocialGraphService.Infrastructure.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly IMongoCollection<Like> _collection;

    public LikeRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Like>("likes");
        
        // Create indexes
        var indexes = new[]
        {
            new CreateIndexModel<Like>(
                Builders<Like>.IndexKeys
                    .Ascending(l => l.UserId)
                    .Ascending(l => l.PostId),
                new CreateIndexOptions { Unique = true }
            ),
            new CreateIndexModel<Like>(
                Builders<Like>.IndexKeys.Ascending(l => l.PostId).Descending(l => l.CreatedAt)
            ),
            new CreateIndexModel<Like>(
                Builders<Like>.IndexKeys.Ascending(l => l.UserId).Descending(l => l.CreatedAt)
            )
        };
        _collection.Indexes.CreateManyAsync(indexes);
    }

    public async Task<Like?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(l => l.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Like?> GetLikeAsync(ObjectId userId, ObjectId postId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(l => l.UserId == userId && l.PostId == postId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasLikedAsync(ObjectId userId, ObjectId postId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(l => l.UserId == userId && l.PostId == postId)
            .AnyAsync(cancellationToken);
    }

    public async Task<long> GetLikeCountAsync(ObjectId postId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .CountDocumentsAsync(l => l.PostId == postId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Like>> FindAsync(ISpecification<Like> specification, CancellationToken cancellationToken = default)
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

    public async Task<long> CountAsync(ISpecification<Like> specification, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(specification.ToFilter(), cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Like entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Like entity, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(
            l => l.Id == entity.Id,
            entity,
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(l => l.Id == id, cancellationToken);
    }
}

using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.PostService.Domain.Entities;
using Sonixy.PostService.Domain.Repositories;
using Sonixy.Shared.Specifications;

namespace Sonixy.PostService.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _collection;

    public PostRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Post>("posts");
        
        // Create indexes
        var indexes = new[]
        {
            new CreateIndexModel<Post>(
                Builders<Post>.IndexKeys.Ascending(p => p.AuthorId).Descending(p => p.CreatedAt)
            ),
            new CreateIndexModel<Post>(
                Builders<Post>.IndexKeys.Descending(p => p.CreatedAt).Descending(p => p.Id)
            ),
            new CreateIndexModel<Post>(
                Builders<Post>.IndexKeys.Ascending(p => p.Visibility).Descending(p => p.CreatedAt)
            )
        };
        _collection.Indexes.CreateManyAsync(indexes);
    }

    public async Task<Post?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> FindAsync(ISpecification<Post> specification, CancellationToken cancellationToken = default)
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

    public async Task<long> CountAsync(ISpecification<Post> specification, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(specification.ToFilter(), cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Post entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Post entity, CancellationToken cancellationToken = default)
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
}

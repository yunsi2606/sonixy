using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.Shared.Common;
using Sonixy.Shared.Specifications;

namespace Sonixy.ChatService.Infrastructure.Repositories;

public abstract class BaseRepository<T>(IMongoDatabase database, string collectionName) : IRepository<T>
    where T : Entity
{
    protected readonly IMongoCollection<T> _collection = database.GetCollection<T>(collectionName);

    public virtual async Task<T?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
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

    public virtual async Task<long> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(specification.ToFilter(), cancellationToken: cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
    }
}

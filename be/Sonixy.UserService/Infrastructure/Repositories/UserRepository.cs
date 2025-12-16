using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.Shared.Common;
using Sonixy.Shared.Specifications;
using Sonixy.UserService.Domain.Entities;
using Sonixy.UserService.Domain.Repositories;

namespace Sonixy.UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<User>("users");
        
        // Create indexes
        var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.Email);
        var indexModel = new CreateIndexModel<User>(indexKeys, new CreateIndexOptions { Unique = true });
        _collection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<User?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(u => u.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _collection
            .Find(u => u.Email == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _collection
            .Find(u => u.Email == normalizedEmail)
            .AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> FindAsync(ISpecification<User> specification, CancellationToken cancellationToken = default)
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

    public async Task<long> CountAsync(ISpecification<User> specification, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(specification.ToFilter(), cancellationToken: cancellationToken);
    }

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(
            u => u.Id == entity.Id,
            entity,
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(u => u.Id == id, cancellationToken);
    }
}

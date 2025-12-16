using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.IdentityService.Domain.Entities;
using Sonixy.IdentityService.Domain.Repositories;
using Sonixy.Shared.Specifications;

namespace Sonixy.IdentityService.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IMongoCollection<Account> _collection;

    public AccountRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Account>("accounts");
        
        // Create indexes
        var indexKeys = Builders<Account>.IndexKeys.Ascending(a => a.Email);
        var indexModel = new CreateIndexModel<Account>(indexKeys, new CreateIndexOptions { Unique = true });
        _collection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<Account?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(a => a.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _collection
            .Find(a => a.Email == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _collection
            .Find(a => a.Email == normalizedEmail)
            .AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Account>> FindAsync(ISpecification<Account> specification, CancellationToken cancellationToken = default)
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

    public async Task<long> CountAsync(ISpecification<Account> specification, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(specification.ToFilter(), cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Account entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Account entity, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(
            a => a.Id == entity.Id,
            entity,
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(a => a.Id == id, cancellationToken);
    }
}

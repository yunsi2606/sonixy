using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.IdentityService.Domain.Entities;
using Sonixy.IdentityService.Domain.Repositories;
using Sonixy.Shared.Specifications;

namespace Sonixy.IdentityService.Infrastructure.Repositories;

public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly IMongoCollection<EmailVerificationToken> _collection;

    public EmailVerificationTokenRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<EmailVerificationToken>("email_verification_tokens");
        
        // Create indexes
        var indexKeys = Builders<EmailVerificationToken>.IndexKeys.Ascending(t => t.Token);
        var indexModel = new CreateIndexModel<EmailVerificationToken>(indexKeys, new CreateIndexOptions { Unique = true });
        _collection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<EmailVerificationToken?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(t => t.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmailVerificationToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(t => t.Token == token)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<EmailVerificationToken>> FindAsync(ISpecification<EmailVerificationToken> specification, CancellationToken cancellationToken = default)
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

    public async Task<long> CountAsync(ISpecification<EmailVerificationToken> specification, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(specification.ToFilter(), cancellationToken: cancellationToken);
    }

    public async Task AddAsync(EmailVerificationToken entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(EmailVerificationToken entity, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(
            t => t.Id == entity.Id,
            entity,
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(t => t.Id == id, cancellationToken);
    }
}

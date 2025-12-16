using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.IdentityService.Domain.Entities;
using Sonixy.IdentityService.Domain.Repositories;
using Sonixy.Shared.Specifications;

namespace Sonixy.IdentityService.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshToken> _collection;

    public RefreshTokenRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<RefreshToken>("refresh_tokens");
        
        // Create indexes
        var indexes = new[]
        {
            new CreateIndexModel<RefreshToken>(
                Builders<RefreshToken>.IndexKeys.Ascending(rt => rt.Token),
                new CreateIndexOptions { Unique = true }
            ),
            new CreateIndexModel<RefreshToken>(
                Builders<RefreshToken>.IndexKeys.Ascending(rt => rt.AccountId)
            ),
            new CreateIndexModel<RefreshToken>(
                Builders<RefreshToken>.IndexKeys.Ascending(rt => rt.ExpiresAt)
            )
        };
        _collection.Indexes.CreateManyAsync(indexes);
    }

    public async Task<RefreshToken?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(rt => rt.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(rt => rt.Token == token)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByAccountAsync(ObjectId accountId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(rt => rt.AccountId == accountId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeTokenAsync(string token, string reason, CancellationToken cancellationToken = default)
    {
        var refreshToken = await GetByTokenAsync(token, cancellationToken);
        
        if (refreshToken is not null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedReason = reason;
            refreshToken.RevokedAt = DateTime.UtcNow;
            
            await UpdateAsync(refreshToken, cancellationToken);
        }
    }

    public async Task RevokeAllAccountTokensAsync(ObjectId accountId, string reason, CancellationToken cancellationToken = default)
    {
        var update = Builders<RefreshToken>.Update
            .Set(rt => rt.IsRevoked, true)
            .Set(rt => rt.RevokedReason, reason)
            .Set(rt => rt.RevokedAt, DateTime.UtcNow);

        await _collection.UpdateManyAsync(
            rt => rt.AccountId == accountId && !rt.IsRevoked,
            update,
            cancellationToken: cancellationToken
        );
    }

    public async Task<IEnumerable<RefreshToken>> FindAsync(ISpecification<RefreshToken> specification, CancellationToken cancellationToken = default)
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

    public async Task<long> CountAsync(ISpecification<RefreshToken> specification, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(specification.ToFilter(), cancellationToken: cancellationToken);
    }

    public async Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(
            rt => rt.Id == entity.Id,
            entity,
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(rt => rt.Id == id, cancellationToken);
    }
}

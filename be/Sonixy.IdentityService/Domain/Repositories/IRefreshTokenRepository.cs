using MongoDB.Bson;
using Sonixy.IdentityService.Domain.Entities;
using Sonixy.Shared.Common;

namespace Sonixy.IdentityService.Domain.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByAccountAsync(ObjectId accountId, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string token, string reason, CancellationToken cancellationToken = default);
    Task RevokeAllAccountTokensAsync(ObjectId accountId, string reason, CancellationToken cancellationToken = default);
}

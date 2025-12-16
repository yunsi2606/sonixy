using MongoDB.Bson;
using Sonixy.IdentityService.Domain.Entities;
using Sonixy.Shared.Common;

namespace Sonixy.IdentityService.Domain.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

using Sonixy.IdentityService.Domain.Entities;
using Sonixy.Shared.Common;

namespace Sonixy.IdentityService.Domain.Repositories;

public interface IEmailVerificationTokenRepository : IRepository<EmailVerificationToken>
{
    Task<EmailVerificationToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}

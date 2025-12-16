using Sonixy.Shared.Common;

namespace Sonixy.Shared.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

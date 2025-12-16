using MongoDB.Driver;

namespace Sonixy.Shared.UnitOfWork;

public class MongoUnitOfWork : IUnitOfWork
{
    private readonly IMongoClient _client;
    private IClientSessionHandle? _session;
    private bool _disposed;

    public MongoUnitOfWork(IMongoClient client)
    {
        _client = client;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
        _session.StartTransaction();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session?.IsInTransaction == true)
        {
            await _session.CommitTransactionAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session?.IsInTransaction == true)
        {
            await _session.AbortTransactionAsync(cancellationToken);
        }
    }

    public Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // MongoDB doesn't require explicit save - operations are immediate
        // This is here for interface compatibility
        return Task.FromResult(true);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _session?.Dispose();
        _disposed = true;
    }
}

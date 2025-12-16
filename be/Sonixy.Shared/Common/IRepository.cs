using MongoDB.Bson;
using Sonixy.Shared.Specifications;

namespace Sonixy.Shared.Common;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<long> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ObjectId id, CancellationToken cancellationToken = default);
}

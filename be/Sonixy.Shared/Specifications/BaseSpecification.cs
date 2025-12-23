using MongoDB.Driver;
using Sonixy.Shared.Common;

namespace Sonixy.Shared.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T> where T : Entity
{
    public FilterDefinition<T> Filter { get; protected set; } = Builders<T>.Filter.Empty;
    public SortDefinition<T>? Sort { get; protected set; }
    public int? Limit { get; protected set; }
    public int? Skip { get; protected set; }

    public FilterDefinition<T> ToFilter() => Filter;
    public SortDefinition<T>? ToSort() => Sort;
}

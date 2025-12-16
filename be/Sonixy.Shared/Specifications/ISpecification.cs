using MongoDB.Driver;
using Sonixy.Shared.Common;

namespace Sonixy.Shared.Specifications;

public interface ISpecification<T> where T : Entity
{
    FilterDefinition<T> ToFilter();
    SortDefinition<T>? ToSort();
    int? Limit { get; }
    int? Skip { get; }
}

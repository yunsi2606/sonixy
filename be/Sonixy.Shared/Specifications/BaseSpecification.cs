using MongoDB.Driver;
using Sonixy.Shared.Common;

namespace Sonixy.Shared.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T> where T : Entity
{
    public FilterDefinition<T> Filter { get; protected set; } = Builders<T>.Filter.Empty;
    public SortDefinition<T>? Sort { get; protected set; }
    public int? Limit { get; protected set; }
    public int? Skip { get; protected set; }

    protected BaseSpecification() { }

    protected BaseSpecification(System.Linq.Expressions.Expression<Func<T, bool>> criteria)
    {
        AddCriteria(criteria);
    }

    protected void AddCriteria(System.Linq.Expressions.Expression<Func<T, bool>> criteria)
    {
        var newFilter = Builders<T>.Filter.Where(criteria);
        if (Filter == Builders<T>.Filter.Empty)
        {
            Filter = newFilter;
        }
        else
        {
            Filter = Builders<T>.Filter.And(Filter, newFilter);
        }
    }

    protected void AddOrderBy(System.Linq.Expressions.Expression<Func<T, object>> orderByExpression)
    {
        Sort = Builders<T>.Sort.Ascending(orderByExpression);
    }

    protected void AddOrderByDescending(System.Linq.Expressions.Expression<Func<T, object>> orderByDescExpression)
    {
        Sort = Builders<T>.Sort.Descending(orderByDescExpression);
    }

    protected void ApplyPaging(int skip, int limit)
    {
        Skip = skip;
        Limit = limit;
    }

    public FilterDefinition<T> ToFilter() => Filter;
    public SortDefinition<T>? ToSort() => Sort;
}

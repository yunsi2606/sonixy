using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.PostService.Domain.Entities;
using Sonixy.Shared.Specifications;

namespace Sonixy.PostService.Application.Specifications;

public class CommentsByPostSpecification(ObjectId postId, string? cursor, int pageSize) : ISpecification<Comment>
{
    public FilterDefinition<Comment> ToFilter()
    {
        var builder = Builders<Comment>.Filter;
        var filter = builder.Eq(c => c.PostId, postId);

        if (!string.IsNullOrEmpty(cursor) && ObjectId.TryParse(cursor, out var cursorId))
        {
            filter = builder.And(filter, builder.Lt(c => c.Id, cursorId));
        }

        return filter;
    }

    public SortDefinition<Comment> ToSort() =>
        Builders<Comment>.Sort.Descending(c => c.CreatedAt).Descending(c => c.Id);

    public int? Limit => pageSize;
    public int? Skip => null;
}

using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.PostService.Domain.Entities;
using Sonixy.Shared.Pagination;
using Sonixy.Shared.Specifications;

namespace Sonixy.PostService.Application.Services;

// Feed specification - public posts ordered by creation time
public class FeedSpecification(string? cursor, int pageSize) : ISpecification<Post>
{
    public FilterDefinition<Post> ToFilter()
    {
        var builder = Builders<Post>.Filter;
        var filter = builder.Eq(p => p.Visibility, "public");

        if (!string.IsNullOrEmpty(cursor))
        {
            var (id, timestamp) = CursorHelper.DecodeCursor(cursor);
            var cursorFilter = builder.And(
                builder.Lt(p => p.CreatedAt, timestamp),
                builder.Ne(p => p.Id, id) // Exclude the cursor item itself
            );
            filter = builder.And(filter, cursorFilter);
        }

        return filter;
    }

    public SortDefinition<Post> ToSort() =>
        Builders<Post>.Sort.Descending(p => p.CreatedAt).Descending(p => p.Id);

    public int? Limit => pageSize;
    public int? Skip => null;
}

// User posts specification - posts by a specific user
public class UserPostsSpecification(ObjectId authorId, string? cursor, int pageSize) : ISpecification<Post>
{
    public FilterDefinition<Post> ToFilter()
    {
        var builder = Builders<Post>.Filter;
        var filter = builder.Eq(p => p.AuthorId, authorId);

        if (!string.IsNullOrEmpty(cursor))
        {
            var (id, timestamp) = CursorHelper.DecodeCursor(cursor);
            var cursorFilter = builder.And(
                builder.Lt(p => p.CreatedAt, timestamp),
                builder.Ne(p => p.Id, id)
            );
            filter = builder.And(filter, cursorFilter);
        }

        return filter;
    }

    public SortDefinition<Post> ToSort() =>
        Builders<Post>.Sort.Descending(p => p.CreatedAt).Descending(p => p.Id);

    public int? Limit => pageSize;
    public int? Skip => null;
}

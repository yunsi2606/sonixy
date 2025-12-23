using MongoDB.Bson;
using MongoDB.Driver;
using Sonixy.PostService.Domain.Entities;
using Sonixy.Shared.Specifications;

namespace Sonixy.PostService.Application.Specifications;

public class PostsByIdsSpecification : BaseSpecification<Post>
{
    public PostsByIdsSpecification(IEnumerable<ObjectId> ids)
    {
        Filter = Builders<Post>.Filter.In(x => x.Id, ids);
    }
}

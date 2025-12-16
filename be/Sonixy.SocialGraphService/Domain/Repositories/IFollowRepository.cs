using MongoDB.Bson;
using Sonixy.Shared.Common;
using Sonixy.SocialGraphService.Domain.Entities;

namespace Sonixy.SocialGraphService.Domain.Repositories;

public interface IFollowRepository : IRepository<Follow>
{
    Task<Follow?> GetFollowAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default);
    Task<bool> IsFollowingAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default);
}

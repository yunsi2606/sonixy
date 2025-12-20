using MongoDB.Bson;
using Sonixy.Shared.Common;
using Sonixy.SocialGraphService.Domain.Entities;

namespace Sonixy.SocialGraphService.Domain.Repositories;

public interface IFollowRepository : IRepository<Follow>
{
    Task<Follow?> GetFollowAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default);
    Task<bool> IsFollowingAsync(ObjectId followerId, ObjectId followingId, CancellationToken cancellationToken = default);
    Task<long> GetFollowerCountAsync(ObjectId userId, CancellationToken cancellationToken = default);
    Task<long> GetFollowingCountAsync(ObjectId userId, CancellationToken cancellationToken = default);
}

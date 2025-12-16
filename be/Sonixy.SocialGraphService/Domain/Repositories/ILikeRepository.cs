using MongoDB.Bson;
using Sonixy.Shared.Common;
using Sonixy.SocialGraphService.Domain.Entities;

namespace Sonixy.SocialGraphService.Domain.Repositories;

public interface ILikeRepository : IRepository<Like>
{
    Task<Like?> GetLikeAsync(ObjectId userId, ObjectId postId, CancellationToken cancellationToken = default);
    Task<bool> HasLikedAsync(ObjectId userId, ObjectId postId, CancellationToken cancellationToken = default);
    Task<long> GetLikeCountAsync(ObjectId postId, CancellationToken cancellationToken = default);
}

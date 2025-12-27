namespace Sonixy.SocialGraphService.Application.Services;

public interface ISocialGraphService
{
    // Follow operations
    Task FollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default);
    Task UnfollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default);
    Task<bool> IsFollowingAsync(string followerId, string followingId, CancellationToken cancellationToken = default);
    
    // Like operations
    Task LikePostAsync(string userId, string postId, CancellationToken cancellationToken = default);
    Task UnlikePostAsync(string userId, string postId, CancellationToken cancellationToken = default);
    Task<long> GetLikeCountAsync(string postId, CancellationToken cancellationToken = default);
    Task<bool> HasLikedAsync(string userId, string postId, CancellationToken cancellationToken = default);
    Task<long> GetFollowersCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<long> GetFollowingCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetFollowersAsync(string userId, int skip = 0, int limit = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetFollowingAsync(string userId, int skip = 0, int limit = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetMutualFollowsAsync(string userId, int skip = 0, int limit = 20, CancellationToken cancellationToken = default);
    Task<bool> IsMutualFollowAsync(string user1, string user2, CancellationToken cancellationToken = default);
}

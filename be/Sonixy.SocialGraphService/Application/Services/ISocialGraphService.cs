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
}

using MongoDB.Bson;
using Sonixy.SocialGraphService.Domain.Entities;
using Sonixy.SocialGraphService.Domain.Repositories;

namespace Sonixy.SocialGraphService.Application.Services;

public class SocialGraphService(IFollowRepository followRepository, ILikeRepository likeRepository)
    : ISocialGraphService
{
    public async Task FollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(followerId, out var followerOid) || 
            !ObjectId.TryParse(followingId, out var followingOid))
            throw new ArgumentException("Invalid user IDs");

        // Check if already following
        if (await followRepository.IsFollowingAsync(followerOid, followingOid, cancellationToken))
            return; // Already following

        var follow = new Follow
        {
            FollowerId = followerOid,
            FollowingId = followingOid
        };

        await followRepository.AddAsync(follow, cancellationToken);
    }

    public async Task UnfollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(followerId, out var followerOid) || 
            !ObjectId.TryParse(followingId, out var followingOid))
            return;

        var follow = await followRepository.GetFollowAsync(followerOid, followingOid, cancellationToken);
        if (follow is not null)
        {
            await followRepository.DeleteAsync(follow.Id, cancellationToken);
        }
    }

    public async Task<bool> IsFollowingAsync(string followerId, string followingId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(followerId, out var followerOid) || 
            !ObjectId.TryParse(followingId, out var followingOid))
            return false;

        return await followRepository.IsFollowingAsync(followerOid, followingOid, cancellationToken);
    }

    public async Task LikePostAsync(string userId, string postId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userOid) || 
            !ObjectId.TryParse(postId, out var postOid))
            throw new ArgumentException("Invalid IDs");

        if (await likeRepository.HasLikedAsync(userOid, postOid, cancellationToken))
            return; // Already liked

        var like = new Like
        {
            UserId = userOid,
            PostId = postOid
        };

        await likeRepository.AddAsync(like, cancellationToken);
    }

    public async Task UnlikePostAsync(string userId, string postId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userOid) || 
            !ObjectId.TryParse(postId, out var postOid))
            return;

        var like = await likeRepository.GetLikeAsync(userOid, postOid, cancellationToken);
        if (like is not null)
        {
            await likeRepository.DeleteAsync(like.Id, cancellationToken);
        }
    }

    public async Task<long> GetLikeCountAsync(string postId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(postId, out var postOid))
            return 0;

        return await likeRepository.GetLikeCountAsync(postOid, cancellationToken);
    }

    public async Task<bool> HasLikedAsync(string userId, string postId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userOid) || 
            !ObjectId.TryParse(postId, out var postOid))
            return false;

        return await likeRepository.HasLikedAsync(userOid, postOid, cancellationToken);
    }

    public async Task<long> GetFollowersCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userOid))
            return 0;

        return await followRepository.GetFollowerCountAsync(userOid, cancellationToken);
    }

    public async Task<long> GetFollowingCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userOid))
            return 0;

        return await followRepository.GetFollowingCountAsync(userOid, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetFollowersAsync(string userId, int skip = 0, int limit = 20, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userOid))
            return [];

        var explicitSkip = Math.Max(0, skip);
        var explicitLimit = Math.Clamp(limit, 1, 100);

        var follows = await followRepository.GetFollowersAsync(userOid, explicitSkip, explicitLimit, cancellationToken);
        return follows.Select(f => f.FollowerId.ToString());
    }

    public async Task<IEnumerable<string>> GetFollowingAsync(string userId, int skip = 0, int limit = 20, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userOid))
            return [];

        var explicitSkip = Math.Max(0, skip);
        var explicitLimit = Math.Clamp(limit, 1, 100);

        var follows = await followRepository.GetFollowingAsync(userOid, explicitSkip, explicitLimit, cancellationToken);
        return follows.Select(f => f.FollowingId.ToString());
    }

    public async Task<IEnumerable<string>> GetMutualFollowsAsync(string userId, int skip = 0, int limit = 20, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userOid))
             return [];
             
        // Get all followers and following (for now simple intersection, might need better query optimizations later)
        // Optimization: Get Following List, then query Check If Following Me for each.
        // Or cleaner: MongoDB Aggregation.
        // Given existing Repo methods, let's fetch Following (usually smaller list) and check reverse relation.
        
        // 1. Get who I follow (limit to 100 or higher for now to find mutuals)
        var myFollowings = await followRepository.GetFollowingAsync(userOid, 0, 1000, cancellationToken);
        var followingIds = myFollowings.Select(f => f.FollowingId).ToList();
        
        var mutuals = new List<string>();
        
        foreach (var fid in followingIds)
        {
            // Check if this person follows me back
            if (await followRepository.IsFollowingAsync(fid, userOid, cancellationToken))
            {
                mutuals.Add(fid.ToString());
            }
        }
        
        return mutuals.Skip(skip).Take(limit);
    }

    public async Task<bool> IsMutualFollowAsync(string user1, string user2, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(user1, out var uid1) || !ObjectId.TryParse(user2, out var uid2))
            return false;

        var follow1 = await followRepository.IsFollowingAsync(uid1, uid2, cancellationToken);
        var follow2 = await followRepository.IsFollowingAsync(uid2, uid1, cancellationToken);

        return follow1 && follow2;
    }
}

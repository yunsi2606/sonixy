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
}

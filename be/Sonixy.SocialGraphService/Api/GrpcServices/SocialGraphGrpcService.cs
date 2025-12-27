using Grpc.Core;
using MongoDB.Bson;
using Sonixy.Shared.Protos;
using Sonixy.SocialGraphService.Application.Services;

namespace Sonixy.SocialGraphService.Api.GrpcServices;

public class SocialGraphGrpcService(ISocialGraphService socialGraphService) 
    : Shared.Protos.SocialGraphService.SocialGraphServiceBase
{
    public override async Task<GetFollowingResponse> GetFollowing(
        GetFollowingRequest request,
        ServerCallContext context)
    {
        if (!ObjectId.TryParse(request.UserId, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID"));
        }

        // TODO: Implement get following list from repository
        // For now, return empty list
        return new GetFollowingResponse();
    }

    public override async Task<GetFollowersResponse> GetFollowers(
        GetFollowersRequest request,
        ServerCallContext context)
    {
        if (!ObjectId.TryParse(request.UserId, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID"));
        }

        // TODO: Implement get followers list from repository
        return new GetFollowersResponse();
    }

    public override async Task<IsFollowingResponse> IsFollowing(
        IsFollowingRequest request,
        ServerCallContext context)
    {
        var isFollowing = await socialGraphService.IsFollowingAsync(
            request.FollowerId,
            request.FollowingId,
            context.CancellationToken);

        return new IsFollowingResponse
        {
            IsFollowing = isFollowing
        };
    }

    public override async Task<GetLikeCountResponse> GetLikeCount(
        GetLikeCountRequest request,
        ServerCallContext context)
    {
        var count = await socialGraphService.GetLikeCountAsync(
            request.PostId,
            context.CancellationToken);

        return new GetLikeCountResponse
        {
            Count = (int)count
        };
    }

    public override async Task<HasLikedResponse> HasLiked(
        HasLikedRequest request,
        ServerCallContext context)
    {
        var hasLiked = await socialGraphService.HasLikedAsync(
            request.UserId,
            request.PostId,
            context.CancellationToken);

        return new HasLikedResponse
        {
            HasLiked = hasLiked
        };
    }

    public override async Task<IsMutualFollowResponse> IsMutualFollow(
        IsMutualFollowRequest request,
        ServerCallContext context)
    {
        var isMutual = await socialGraphService.IsMutualFollowAsync(
            request.UserId1,
            request.UserId2,
            context.CancellationToken);

        return new IsMutualFollowResponse
        {
            IsMutual = isMutual
        };
    }
}

using Grpc.Core;
using Sonixy.PostService.Application.Services;
using Google.Protobuf.WellKnownTypes;
using Sonixy.Shared.Protos;

namespace Sonixy.PostService.Api.Services;

public class PostGrpcService(IPostService postService, ILogger<PostGrpcService> logger) : Shared.Protos.PostService.PostServiceBase
{
    public override async Task<PostListResponse> GetPostsByIds(GetPostsByIdsRequest request, ServerCallContext context)
    {
        var posts = await postService.GetPostsByIdsAsync(request.PostIds, cancellationToken: context.CancellationToken);
        
        var response = new PostListResponse();
        response.Posts.AddRange(posts.Select(p => new PostResponse
        {
            Id = p.Id,
            AuthorId = p.AuthorId,
            Content = p.Content,
            AuthorDisplayName = p.AuthorDisplayName ?? "",
            AuthorUsername = p.AuthorUsername ?? "",
            AuthorAvatarUrl = p.AuthorAvatarUrl ?? "",
            LikeCount = p.LikeCount,
            IsLiked = p.IsLiked,
            CreatedAt = Timestamp.FromDateTime(p.CreatedAt.ToUniversalTime()),
            Media = { p.Media.Select(m => new MediaItemResponse { Type = m.Type, Url = m.Url }) }
        }));

        return response;
    }
}

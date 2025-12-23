using Sonixy.Shared.Protos;

namespace Sonixy.FeedService.Services;

public interface IPostClient
{
    Task<List<DTOs.PostDto>> GetPostsByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
}

public class PostClient(PostService.PostServiceClient grpcClient, ILogger<PostClient> logger) : IPostClient
{
    public async Task<List<DTOs.PostDto>> GetPostsByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        try 
        {
            var request = new GetPostsByIdsRequest();
            request.PostIds.AddRange(ids);

            var reply = await grpcClient.GetPostsByIdsAsync(request, cancellationToken: cancellationToken);
            
            return reply.Posts.Select(p => new DTOs.PostDto(
                Id: p.Id,
                AuthorId: p.AuthorId,
                AuthorDisplayName: p.AuthorDisplayName,
                AuthorAvatarUrl: p.AuthorAvatarUrl,
                AuthorUsername: p.AuthorUsername,
                Content: p.Content,
                Visibility: "public",
                LikeCount: p.LikeCount,
                IsLiked: p.IsLiked,
                Media: p.Media.Select(m => new DTOs.MediaItemDto(m.Type, m.Url)).ToList(),
                CreatedAt: p.CreatedAt.ToDateTime(),
                UpdatedAt: p.CreatedAt.ToDateTime()
            )).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching posts via gRPC");
            return [];
        }
    }
}

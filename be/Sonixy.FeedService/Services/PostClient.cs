using Sonixy.FeedService.DTOs;

namespace Sonixy.FeedService.Services;

public interface IPostClient
{
    Task<List<PostDto>> GetPostsByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
}

public class PostClient(HttpClient httpClient, ILogger<PostClient> logger) : IPostClient
{
    public async Task<List<PostDto>> GetPostsByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        try 
        {
            var response = await httpClient.PostAsJsonAsync("/api/posts/batch", ids, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                 logger.LogWarning("Failed to fetch posts batch. Status: {StatusCode}", response.StatusCode);
                 return [];
            }
            return await response.Content.ReadFromJsonAsync<List<PostDto>>(cancellationToken) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching posts batch");
            return [];
        }
    }
}

using Sonixy.Shared.Protos;

namespace Sonixy.FeedService.Services;

public interface ISocialClient
{
    Task<List<string>> GetFollowerIdsAsync(string userId, CancellationToken cancellationToken = default);
}

public class SocialClient(SocialGraphService.SocialGraphServiceClient grpcClient, ILogger<SocialClient> logger) : ISocialClient
{
    public async Task<List<string>> GetFollowerIdsAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetFollowersRequest
            {
                UserId = userId,
                Limit = 5000 // Cap fan-out at 5000 followers per post
            };

            var response = await grpcClient.GetFollowersAsync(request, cancellationToken: cancellationToken);
            return response.UserIds.ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get followers for user {UserId} via gRPC", userId);
            return [];
        }
    }
}

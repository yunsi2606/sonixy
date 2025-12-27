using Microsoft.AspNetCore.Http;
using Sonixy.ChatService.Application.Interfaces;
using Sonixy.Shared.Protos;

namespace Sonixy.ChatService.Infrastructure.Services;

public class SocialGraphClient(
    SocialGraphService.SocialGraphServiceClient grpcClient, 
    IHttpContextAccessor httpContextAccessor) 
    : ISocialGraphClient
{
    public async Task<bool> IsMutualFollowAsync(string targetUserId)
    {
        try
        {
            var context = httpContextAccessor.HttpContext;
            var currentUserId = context?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(currentUserId)) return false;

            var response = await grpcClient.IsMutualFollowAsync(new IsMutualFollowRequest
            {
                UserId1 = currentUserId,
                UserId2 = targetUserId
            });

            return response.IsMutual;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking mutual status via gRPC: {ex.Message}");
            return false;
        }
    }
}

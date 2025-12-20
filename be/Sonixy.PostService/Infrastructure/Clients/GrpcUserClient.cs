using Grpc.Core;
using Microsoft.Extensions.Logging;
using Sonixy.PostService.Application.Interfaces;
using Sonixy.Shared.Protos;
using UserDto = Sonixy.PostService.Application.DTOs.UserDto;

namespace Sonixy.PostService.Infrastructure.Clients;

public class GrpcUserClient(
    UserService.UserServiceClient grpcClient,
    ILogger<GrpcUserClient> logger) : IUserClient
{
    public async Task<UserDto?> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetUserRequest { UserId = userId };
            var response = await grpcClient.GetUserAsync(request, cancellationToken: cancellationToken);
            
            return new UserDto(
                response.Id,
                response.DisplayName,
                response.AvatarUrl
            );
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch user {UserId} via gRPC", userId);
            return null;
        }
    }

    public async Task<IEnumerable<UserDto>> GetUsersBatchAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetUsersBatchRequest();
            request.UserIds.AddRange(userIds);
            
            var response = await grpcClient.GetUsersBatchAsync(request, cancellationToken: cancellationToken);
            
            return response.Users.Select(u => new UserDto(
                u.Id,
                u.DisplayName,
                u.AvatarUrl
            )).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch batch users via gRPC");
            return [];
        }
    }
}

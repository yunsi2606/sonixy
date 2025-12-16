using System.Globalization;
using Grpc.Core;
using MongoDB.Bson;
using Sonixy.Shared.Protos;
using Sonixy.UserService.Application.Services;

namespace Sonixy.UserService.Api.GrpcServices;

public class UserGrpcService(IUserService userService) : Shared.Protos.UserService.UserServiceBase
{
    public override async Task<GetUserResponse> GetUser(
        GetUserRequest request,
        ServerCallContext context)
    {
        var user = await userService.GetUserByIdAsync(request.UserId, context.CancellationToken);
        
        if (user is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        return new GetUserResponse
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt.ToString(CultureInfo.CurrentCulture)
        };
    }

    public override async Task<GetUsersBatchResponse> GetUsersBatch(
        GetUsersBatchRequest request,
        ServerCallContext context)
    {
        var users = await userService.GetUsersBatchAsync(
            request.UserIds.ToList(), 
            context.CancellationToken);

        var response = new GetUsersBatchResponse();
        
        foreach (var user in users)
        {
            response.Users.Add(new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl
            });
        }

        return response;
    }

    public override async Task<UserExistsResponse> UserExists(
        UserExistsRequest request,
        ServerCallContext context)
    {
        var user = await userService.GetUserByIdAsync(request.UserId, context.CancellationToken);
        
        return new UserExistsResponse
        {
            Exists = user is not null
        };
    }
}

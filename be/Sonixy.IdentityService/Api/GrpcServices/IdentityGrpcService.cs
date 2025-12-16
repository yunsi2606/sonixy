using Grpc.Core;
using MongoDB.Bson;
using Sonixy.IdentityService.Application.Services;
using Sonixy.IdentityService.Domain.Repositories;
using Sonixy.Shared.Protos;

namespace Sonixy.IdentityService.Api.GrpcServices;

public class IdentityGrpcService(
    ITokenService tokenService,
    IAccountRepository accountRepository) : Shared.Protos.IdentityService.IdentityServiceBase
{
    public override async Task<ValidateTokenResponse> ValidateToken(
        ValidateTokenRequest request,
        ServerCallContext context)
    {
        try
        {
            var userId = tokenService.GetUserIdFromToken(request.AccessToken);
            
            if (string.IsNullOrEmpty(userId))
            {
                return new ValidateTokenResponse
                {
                    IsValid = false,
                    ErrorMessage = "Invalid token"
                };
            }

            return new ValidateTokenResponse
            {
                IsValid = true,
                UserId = userId
            };
        }
        catch (Exception ex)
        {
            return new ValidateTokenResponse
            {
                IsValid = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public override async Task<GetUserClaimsResponse> GetUserClaims(
        GetUserClaimsRequest request,
        ServerCallContext context)
    {
        if (!ObjectId.TryParse(request.UserId, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID"));
        }

        var account = await accountRepository.GetByIdAsync(userId, context.CancellationToken);
        
        if (account is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));
        }

        return new GetUserClaimsResponse
        {
            UserId = account.Id.ToString(),
            Email = account.Email,
            IsActive = account.IsActive
        };
    }
}

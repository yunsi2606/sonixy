using Sonixy.PostService.Application.DTOs;

namespace Sonixy.PostService.Application.Interfaces;

public interface IUserClient
{
    Task<UserDto?> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetUsersBatchAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);
    Task<string?> GetUserIdByUsernameAsync(string username, CancellationToken cancellationToken = default);
}



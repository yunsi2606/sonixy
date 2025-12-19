using MongoDB.Bson;
using Sonixy.UserService.Application.DTOs;

namespace Sonixy.UserService.Application.Services;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(string id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetUsersBatchAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<(string UploadUrl, string ObjectKey, string PublicUrl)> GeneratePresignedUrlAsync(string fileName, string contentType, CancellationToken cancellationToken = default);
}

using MongoDB.Bson;
using Sonixy.UserService.Application.DTOs;
using Sonixy.UserService.Domain.Entities;
using Sonixy.UserService.Domain.Repositories;
using Sonixy.Shared.Specifications;
using Sonixy.Shared.Interfaces;
using MongoDB.Driver;

namespace Sonixy.UserService.Application.Services;

public class UserService(IUserRepository userRepository, IMediaStorage mediaStorage) : IUserService
{
    public async Task<(string UploadUrl, string ObjectKey, string PublicUrl)> GeneratePresignedUrlAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        return await mediaStorage.GeneratePresignedUploadUrlAsync(fileName, contentType, cancellationToken);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        // Check if email already exists
        if (await userRepository.EmailExistsAsync(dto.Email, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists");
        }

        // Check if username already exists
        if (await userRepository.UsernameExistsAsync(dto.Username, cancellationToken))
        {
            throw new InvalidOperationException("Username already exists");
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email.ToLowerInvariant(),
            Username = dto.Username,
            Bio = dto.Bio ?? string.Empty,
            AvatarUrl = dto.AvatarUrl ?? string.Empty
        };

        if (!string.IsNullOrEmpty(dto.Id) && ObjectId.TryParse(dto.Id, out var objectId))
        {
            user.Id = objectId;
        }

        await userRepository.AddAsync(user, cancellationToken);

        return MapToDto(user);
    }

    public async Task<UserDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var user = await userRepository.GetByIdAsync(objectId, cancellationToken);
        return user is not null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        return user is not null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByUsernameAsync(username, cancellationToken);
        return user is not null ? MapToDto(user) : null;
    }

    public async Task<bool> IsUsernameAvailableAsync(string username, CancellationToken cancellationToken = default)
    {
        return !await userRepository.UsernameExistsAsync(username, cancellationToken);
    }

    public async Task<UserDto> UpdateUserAsync(string id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            throw new ArgumentException("Invalid user ID");

        var user = await userRepository.GetByIdAsync(objectId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException("User not found");

        if (dto.FirstName is not null)
            user.FirstName = dto.FirstName;

        if (dto.LastName is not null)
            user.LastName = dto.LastName;

        if (dto.Bio is not null)
            user.Bio = dto.Bio;

        if (dto.AvatarUrl is not null)
            user.AvatarUrl = dto.AvatarUrl;

        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        return MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetUsersBatchAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var objectIds = ids
            .Select(id => ObjectId.TryParse(id, out var oid) ? oid : (ObjectId?)null)
            .Where(oid => oid.HasValue)
            .Select(oid => oid!.Value)
            .ToList();

        var spec = new UserBatchSpecification(objectIds);
        var users = await userRepository.FindAsync(spec, cancellationToken);

        return users.Select(MapToDto);
    }

    private static UserDto MapToDto(User user) => new(
        user.Id.ToString(),
        user.FirstName,
        user.LastName,
        user.DisplayName,
        user.Email,
        user.Username,
        user.Bio,
        user.AvatarUrl,
        user.CreatedAt
    );
}

// Specification for batch user query
internal class UserBatchSpecification(IEnumerable<ObjectId> ids) : ISpecification<User>
{
    public FilterDefinition<User> ToFilter() =>
        Builders<User>.Filter.In(u => u.Id, ids);

    public SortDefinition<User>? ToSort() => null;
    public int? Limit => null;
    public int? Skip => null;
}

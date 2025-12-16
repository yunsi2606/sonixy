using MongoDB.Bson;
using Sonixy.UserService.Application.DTOs;
using Sonixy.UserService.Domain.Entities;
using Sonixy.UserService.Domain.Repositories;
using Sonixy.Shared.Specifications;
using MongoDB.Driver;

namespace Sonixy.UserService.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        // Check if email already exists
        if (await userRepository.EmailExistsAsync(dto.Email, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var user = new User
        {
            DisplayName = dto.DisplayName,
            Email = dto.Email.ToLowerInvariant(),
            Bio = dto.Bio ?? string.Empty,
            AvatarUrl = dto.AvatarUrl ?? string.Empty
        };

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

    public async Task<UserDto> UpdateUserAsync(string id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            throw new ArgumentException("Invalid user ID");

        var user = await userRepository.GetByIdAsync(objectId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException("User not found");

        if (dto.DisplayName is not null)
            user.DisplayName = dto.DisplayName;

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
        user.DisplayName,
        user.Email,
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

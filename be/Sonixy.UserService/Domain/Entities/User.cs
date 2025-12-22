using MongoDB.Bson.Serialization.Attributes;
using Sonixy.Shared.Common;

namespace Sonixy.UserService.Domain.Entities;

[BsonIgnoreExtraElements]
public class User : Entity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string DisplayName => !string.IsNullOrWhiteSpace(FirstName) ? $"{FirstName} {LastName}".Trim() : Username;
    public required string Email { get; set; }
    public required string Username { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
}

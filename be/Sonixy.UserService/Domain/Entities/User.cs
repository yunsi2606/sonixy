using MongoDB.Bson;
using Sonixy.Shared.Common;

namespace Sonixy.UserService.Domain.Entities;

public class User : Entity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string DisplayName => !string.IsNullOrEmpty(FirstName) ? $"{FirstName} {LastName}" : "Unknown User";
    public required string Email { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}

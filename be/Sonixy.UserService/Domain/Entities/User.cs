using MongoDB.Bson;
using Sonixy.Shared.Common;

namespace Sonixy.UserService.Domain.Entities;

public class User : Entity
{
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}

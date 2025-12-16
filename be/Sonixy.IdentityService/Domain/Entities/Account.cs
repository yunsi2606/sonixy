using MongoDB.Bson;
using Sonixy.Shared.Common;

namespace Sonixy.IdentityService.Domain.Entities;

public class Account : Entity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
}

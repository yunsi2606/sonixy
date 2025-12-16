using MongoDB.Bson;
using Sonixy.Shared.Common;

namespace Sonixy.IdentityService.Domain.Entities;

public class RefreshToken : Entity
{
    public required ObjectId AccountId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public string? RevokedReason { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
}

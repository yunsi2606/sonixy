using MongoDB.Bson;
using Sonixy.Shared.Common;

namespace Sonixy.IdentityService.Domain.Entities;

public class EmailVerificationToken : Entity
{
    public ObjectId AccountId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}

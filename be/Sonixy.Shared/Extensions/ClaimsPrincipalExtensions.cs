using System.Security.Claims;
using MongoDB.Bson;

namespace Sonixy.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the NameIdentifier claim.
    /// </summary>
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Gets the user ID as an ObjectId from the NameIdentifier claim.
    /// </summary>
    public static ObjectId? GetUserObjectId(this ClaimsPrincipal principal)
    {
        return principal.GetUserId().ToObjectId();
    }
}

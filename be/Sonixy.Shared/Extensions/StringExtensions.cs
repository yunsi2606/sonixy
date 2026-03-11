    using MongoDB.Bson;

namespace Sonixy.Shared.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Attempts to parse a string into an ObjectId. Returns null if invalid or null/empty.
    /// </summary>
    public static ObjectId? ToObjectId(this string? id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return ObjectId.TryParse(id, out var objectId) ? objectId : null;
    }

    /// <summary>
    /// Attempts to parse a string into an ObjectId. Throws ArgumentException if invalid.
    /// </summary>
    public static ObjectId ToObjectIdRequired(this string? id, string paramName = "id")
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(paramName);
        if (ObjectId.TryParse(id, out var objectId))
        {
            return objectId;
        }
        throw new ArgumentException($"Invalid ObjectId format: {id}", paramName);
    }
}

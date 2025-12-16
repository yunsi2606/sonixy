using System.Text;
using MongoDB.Bson;

namespace Sonixy.Shared.Pagination;

public static class CursorHelper
{
    public static string EncodeCursor(ObjectId id, DateTime timestamp)
    {
        var data = $"{id}:{timestamp.Ticks}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
    }

    public static (ObjectId Id, DateTime Timestamp) DecodeCursor(string cursor)
    {
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
        var parts = decoded.Split(':');
        
        if (parts.Length != 2)
            throw new ArgumentException("Invalid cursor format");

        var id = ObjectId.Parse(parts[0]);
        var timestamp = new DateTime(long.Parse(parts[1]));
        
        return (id, timestamp);
    }
}

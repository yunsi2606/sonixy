using Nest;

namespace Sonixy.SearchService.Infrastructure.Elasticsearch;

/// <summary>
/// Elasticsearch index model for User documents
/// </summary>
[ElasticsearchType(IdProperty = nameof(Id))]
public class UserDocument
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public DateTime IndexedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Elasticsearch index model for Post documents
/// </summary>
[ElasticsearchType(IdProperty = nameof(Id))]
public class PostDocument
{
    public string Id { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Hashtags { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime IndexedAt { get; set; } = DateTime.UtcNow;
}

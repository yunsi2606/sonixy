using Microsoft.Extensions.Logging;
using Nest;
using Sonixy.SearchService.Application.DTOs;
using Sonixy.SearchService.Infrastructure.Elasticsearch;

namespace Sonixy.SearchService.Application.Services;

public class ElasticsearchSearchService : ISearchService
{
    private readonly IElasticClient _client;
    private readonly ILogger<ElasticsearchSearchService> _logger;
    private const string UsersIndex = "sonixy-users";
    private const string PostsIndex = "sonixy-posts";

    public ElasticsearchSearchService(IElasticClient client, ILogger<ElasticsearchSearchService> logger)
    {
        _client = client;
        _logger = logger;
        EnsureIndicesCreated().GetAwaiter().GetResult();
    }

    private async Task EnsureIndicesCreated()
    {
        // Create users index if not exists
        var usersExists = await _client.Indices.ExistsAsync(UsersIndex);
        if (!usersExists.Exists)
        {
            await _client.Indices.CreateAsync(UsersIndex, c => c
                .Map<UserDocument>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Text(t => t.Name(n => n.Username).Analyzer("standard"))
                        .Text(t => t.Name(n => n.DisplayName).Analyzer("standard"))
                        .Text(t => t.Name(n => n.Bio).Analyzer("standard"))
                    )
                )
            );
            _logger.LogInformation("Created Elasticsearch index: {Index}", UsersIndex);
        }

        // Create posts index if not exists
        var postsExists = await _client.Indices.ExistsAsync(PostsIndex);
        if (!postsExists.Exists)
        {
            await _client.Indices.CreateAsync(PostsIndex, c => c
                .Map<PostDocument>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Text(t => t.Name(n => n.Content).Analyzer("standard"))
                        .Keyword(k => k.Name(n => n.Hashtags))
                    )
                )
            );
            _logger.LogInformation("Created Elasticsearch index: {Index}", PostsIndex);
        }
    }

    public async Task<List<UserSearchResultDto>> SearchUsersAsync(string query, int limit = 20, CancellationToken ct = default)
    {
        var response = await _client.SearchAsync<UserDocument>(s => s
            .Index(UsersIndex)
            .Size(limit)
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field(ff => ff.Username, boost: 3)
                        .Field(ff => ff.DisplayName, boost: 2)
                        .Field(ff => ff.Bio)
                    )
                    .Query(query)
                    .Fuzziness(Fuzziness.Auto)
                )
            ), ct
        );

        if (!response.IsValid)
        {
            _logger.LogWarning("Elasticsearch user search failed: {Error}", response.DebugInformation);
            return [];
        }

        return response.Documents.Select(d => new UserSearchResultDto(
            d.Id,
            d.Username,
            d.DisplayName,
            d.AvatarUrl,
            d.Bio
        )).ToList();
    }

    public async Task<List<PostSearchResultDto>> SearchPostsAsync(string query, int limit = 20, CancellationToken ct = default)
    {
        var response = await _client.SearchAsync<PostDocument>(s => s
            .Index(PostsIndex)
            .Size(limit)
            .Sort(ss => ss.Descending(p => p.CreatedAt))
            .Query(q => q
                .Bool(b => b
                    .Should(
                        sh => sh.Match(m => m.Field(f => f.Content).Query(query)),
                        sh => sh.Term(t => t.Field(f => f.Hashtags).Value(query.TrimStart('#').ToLower()))
                    )
                )
            ), ct
        );

        if (!response.IsValid)
        {
            _logger.LogWarning("Elasticsearch post search failed: {Error}", response.DebugInformation);
            return [];
        }

        var posts = response.Documents.ToList();
        if (posts.Count == 0) return [];

        var authorIds = posts.Select(p => p.AuthorId).Distinct().ToList();

        // Fetch authors from UsersIndex
        var usersResponse = await _client.SearchAsync<UserDocument>(s => s
            .Index(UsersIndex)
            .Size(authorIds.Count)
            .Query(q => q
                .Terms(t => t.Field(f => f.Id.Suffix("keyword")).Terms(authorIds))
            ), ct
        );

        var authorMap = usersResponse.IsValid 
            ? usersResponse.Documents.ToDictionary(u => u.Id, u => u) 
            : new Dictionary<string, UserDocument>();

        return posts.Select(d =>
        {
            var author = authorMap.GetValueOrDefault(d.AuthorId);
            return new PostSearchResultDto(
                d.Id,
                d.AuthorId,
                author?.Username ?? string.Empty,
                author?.DisplayName ?? "Unknown User",
                author?.AvatarUrl ?? string.Empty,
                d.Content,
                d.Hashtags,
                d.CreatedAt
            );
        }).ToList();
    }

    public async Task IndexUserAsync(string id, string username, string displayName, string avatarUrl, string bio, CancellationToken ct = default)
    {
        var document = new UserDocument
        {
            Id = id,
            Username = username,
            DisplayName = displayName,
            AvatarUrl = avatarUrl,
            Bio = bio,
            IndexedAt = DateTime.UtcNow
        };

        var response = await _client.IndexAsync(document, i => i.Index(UsersIndex).Id(id), ct);
        if (!response.IsValid)
        {
            _logger.LogError("Failed to index user {UserId}: {Error}", id, response.DebugInformation);
        }
        else
        {
            _logger.LogInformation("Indexed user {UserId}", id);
        }
    }

    public async Task IndexPostAsync(string id, string authorId, string content, List<string> hashtags, DateTime createdAt, CancellationToken ct = default)
    {
        var document = new PostDocument
        {
            Id = id,
            AuthorId = authorId,
            Content = content,
            Hashtags = hashtags,
            CreatedAt = createdAt,
            IndexedAt = DateTime.UtcNow
        };

        var response = await _client.IndexAsync(document, i => i.Index(PostsIndex).Id(id), ct);
        if (!response.IsValid)
        {
            _logger.LogError("Failed to index post {PostId}: {Error}", id, response.DebugInformation);
        }
        else
        {
            _logger.LogInformation("Indexed post {PostId} with {HashtagCount} hashtags", id, hashtags.Count);
        }
    }
}

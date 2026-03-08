using Sonixy.SearchService.Application.DTOs;

namespace Sonixy.SearchService.Application.Services;

public interface ISearchService
{
    Task<List<UserSearchResultDto>> SearchUsersAsync(string query, int limit = 20, CancellationToken ct = default);
    Task<List<PostSearchResultDto>> SearchPostsAsync(string query, int limit = 20, CancellationToken ct = default);
    Task IndexUserAsync(string id, string username, string displayName, string avatarUrl, string bio, CancellationToken ct = default);
    Task IndexPostAsync(string id, string authorId, string content, List<string> hashtags, DateTime createdAt, CancellationToken ct = default);
}

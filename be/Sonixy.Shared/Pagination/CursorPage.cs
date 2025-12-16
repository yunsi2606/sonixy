namespace Sonixy.Shared.Pagination;

public record CursorPage<T>(
    IEnumerable<T> Items,
    string? NextCursor,
    bool HasMore
);

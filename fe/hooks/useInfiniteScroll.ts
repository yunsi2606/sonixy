'use client';

import { useState, useEffect, useCallback } from 'react';

interface InfiniteScrollOptions<T> {
    fetchFunction: (cursor?: string) => Promise<{ items: T[]; nextCursor: string | null; hasMore: boolean }>;
    pageSize?: number;
}

export function useInfiniteScroll<T>({ fetchFunction, pageSize = 20 }: InfiniteScrollOptions<T>) {
    const [items, setItems] = useState<T[]>([]);
    const [cursor, setCursor] = useState<string | null>(null);
    const [hasMore, setHasMore] = useState(true);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const loadMore = useCallback(async () => {
        if (isLoading || !hasMore) return;

        setIsLoading(true);
        setError(null);

        try {
            const result = await fetchFunction(cursor || undefined);
            setItems((prev) => [...prev, ...result.items]);
            setCursor(result.nextCursor);
            setHasMore(result.hasMore);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to load');
        } finally {
            setIsLoading(false);
        }
    }, [cursor, hasMore, isLoading, fetchFunction]);

    const refresh = useCallback(async () => {
        setItems([]);
        setCursor(null);
        setHasMore(true);
        setIsLoading(true);
        setError(null);

        try {
            const result = await fetchFunction(undefined);
            setItems(result.items);
            setCursor(result.nextCursor);
            setHasMore(result.hasMore);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to refresh');
        } finally {
            setIsLoading(false);
        }
    }, [fetchFunction]);

    // Load initial data
    useEffect(() => {
        loadMore();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return {
        items,
        hasMore,
        isLoading,
        error,
        loadMore,
        refresh,
    };
}

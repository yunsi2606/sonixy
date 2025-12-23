import { apiClient } from '@/lib/api';
import { Post } from '@/types/api';

const FEED_BASE = '/api/feed';

export interface FeedResponse {
    items: Post[];
    hasMore: boolean;
}

export const feedService = {
    async getTimeline(userId: string): Promise<FeedResponse> {
        if (!userId) return { items: [], hasMore: false };
        return apiClient.get<FeedResponse>(`${FEED_BASE}?userId=${userId}`);
    },

    async getUserNetwork(userId: string): Promise<any> {
        return apiClient.get(`${FEED_BASE}/network/${userId}`);
    }
};

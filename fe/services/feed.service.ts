import { apiClient } from '@/lib/api';
import { Post } from '@/types/api';

const FEED_BASE = '/api/feed';

export interface FeedResponse {
    items: Post[];
    hasMore: boolean;
}

export const feedService = {
    async getTimeline(page: number = 1, pageSize: number = 20): Promise<FeedResponse> {
        return apiClient.get<FeedResponse>(`${FEED_BASE}?page=${page}&pageSize=${pageSize}`);
    },

    async getUserNetwork(userId: string): Promise<any> {
        return apiClient.get(`${FEED_BASE}/network/${userId}`);
    }
};

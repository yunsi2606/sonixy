import { apiClient } from '@/lib/api';
import { authService } from './auth.service';

const SOCIAL_BASE = '/api/social'; // Mapped via Gateway

export const socialService = {
    // Follows
    async followUser(userId: string): Promise<void> {
        const token = authService.getAccessToken(); // Ensure token is used
        if (!token) throw new Error('Not authenticated');
        await apiClient.post(`${SOCIAL_BASE}/follows/${userId}`, {}, token);
    },

    async unfollowUser(userId: string): Promise<void> {
        const token = authService.getAccessToken();
        if (!token) throw new Error('Not authenticated');
        await apiClient.delete(`${SOCIAL_BASE}/follows/${userId}`, token);
    },

    async getFollowStatus(userId: string): Promise<boolean> {
        const token = authService.getAccessToken();
        if (!token) return false;
        const res = await apiClient.get<{ isFollowing: boolean }>(`${SOCIAL_BASE}/follows/${userId}/status`, token);
        return res.isFollowing;
    },

    async getFollowersCount(userId: string): Promise<number> {
        const res = await apiClient.get<{ count: number }>(`${SOCIAL_BASE}/follows/${userId}/followers/count`);
        return res.count;
    },

    async getFollowingCount(userId: string): Promise<number> {
        const res = await apiClient.get<{ count: number }>(`${SOCIAL_BASE}/follows/${userId}/following/count`);
        return res.count;
    },

    async getFollowers(userId: string, skip = 0, limit = 20): Promise<string[]> {
        return apiClient.get<string[]>(`${SOCIAL_BASE}/follows/${userId}/followers?skip=${skip}&limit=${limit}`);
    },

    async getFollowing(userId: string, skip = 0, limit = 20): Promise<string[]> {
        return apiClient.get<string[]>(`${SOCIAL_BASE}/follows/${userId}/following?skip=${skip}&limit=${limit}`);
    },

    // Likes
    async likePost(postId: string): Promise<void> {
        const token = authService.getAccessToken();
        if (!token) throw new Error('Not authenticated');
        await apiClient.post(`${SOCIAL_BASE}/likes/${postId}`, {}, token);
    },

    async unlikePost(postId: string): Promise<void> {
        const token = authService.getAccessToken();
        if (!token) throw new Error('Not authenticated');
        await apiClient.delete(`${SOCIAL_BASE}/likes/${postId}`, token);
    },

    async getLikeCount(postId: string): Promise<number> {
        const res = await apiClient.get<{ count: number }>(`${SOCIAL_BASE}/likes/${postId}/count`);
        return res.count;
    }
};

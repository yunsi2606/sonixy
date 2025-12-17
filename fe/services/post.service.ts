import { apiClient } from '@/lib/api';
import type { Post, CursorPage } from '@/types/api';

export const postService = {
    async getFeed(cursor?: string): Promise<CursorPage<Post>> {
        const query = cursor ? `?cursor=${cursor}` : '';
        return apiClient.get<CursorPage<Post>>(`/api/posts/feed${query}`);
    },

    async getUserPosts(userId: string, cursor?: string): Promise<CursorPage<Post>> {
        const query = cursor ? `?cursor=${cursor}` : '';
        return apiClient.get<CursorPage<Post>>(`/api/posts/user/${userId}${query}`);
    },

    async createPost(content: string, visibility: 'public' | 'followers' = 'public'): Promise<Post> {
        return apiClient.post<Post>('/api/posts', { content, visibility });
    },

    async toggleLike(postId: string): Promise<{ success: boolean }> {
        return apiClient.post<{ success: boolean }>(`/api/posts/${postId}/like`, {});
    }
};

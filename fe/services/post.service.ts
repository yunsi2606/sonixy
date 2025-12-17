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

    async createPost(content: string, visibility: 'public' | 'followers' = 'public', files?: File[]): Promise<Post> {
        const formData = new FormData();
        formData.append('content', content);
        formData.append('visibility', visibility);

        if (files) {
            files.forEach(file => formData.append('media', file));
        }

        // Note: ApiClient needs update to handle FormData, or we use fetch directly here for specific override
        // Assuming ApiClient handles it or we bypass it partially. 
        // Let's modify ApiClient or just use a specialized call. 
        // For simplicity, adapting ApiClient.post to check if data is FormData.

        return apiClient.post<Post>('/api/posts', formData);
    },

    async toggleLike(postId: string): Promise<{ success: boolean }> {
        return apiClient.post<{ success: boolean }>(`/api/posts/${postId}/like`, {});
    }
};

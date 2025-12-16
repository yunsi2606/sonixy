import { apiClient } from '@/lib/api';
import { authService } from './auth.service';
import type { Post, CursorPage } from '@/types/api';

const POST_BASE = '/api/posts';

export interface CreatePostDto {
    content: string;
    visibility: 'public' | 'followers';
}

export const postService = {
    async createPost(data: CreatePostDto): Promise<Post> {
        const token = authService.getAccessToken();
        return apiClient.post<Post>(`${POST_BASE}`, data, token || undefined);
    },

    async getPost(id: string): Promise<Post> {
        const token = authService.getAccessToken();
        return apiClient.get<Post>(`${POST_BASE}/${id}`, token || undefined);
    },

    async getFeed(cursor?: string, pageSize: number = 20): Promise<CursorPage<Post>> {
        const token = authService.getAccessToken();
        const params = new URLSearchParams();
        if (cursor) params.set('cursor', cursor);
        params.set('pageSize', pageSize.toString());

        return apiClient.get<CursorPage<Post>>(
            `${POST_BASE}/feed?${params.toString()}`,
            token || undefined
        );
    },

    async getUserPosts(userId: string, cursor?: string, pageSize: number = 20): Promise<CursorPage<Post>> {
        const token = authService.getAccessToken();
        const params = new URLSearchParams();
        if (cursor) params.set('cursor', cursor);
        params.set('pageSize', pageSize.toString());

        return apiClient.get<CursorPage<Post>>(
            `${POST_BASE}/user/${userId}?${params.toString()}`,
            token || undefined
        );
    },
};

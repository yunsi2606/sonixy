import { apiClient } from '@/lib/api';
import type { Comment, CreateCommentDto } from '@/types/comment';
import type { CursorPage } from '@/types/api';
import { authService } from './auth.service';

const BASE_URL = '/api';

export const commentService = {
    async getComments(postId: string, cursor?: string): Promise<CursorPage<Comment>> {
        const query = new URLSearchParams({ postId });
        if (cursor) query.append('cursor', cursor);
        return apiClient.get<CursorPage<Comment>>(`${BASE_URL}/posts/${postId}/comments?${query.toString()}`);
    },

    async createComment(dto: CreateCommentDto): Promise<Comment> {
        return apiClient.post<Comment>(`${BASE_URL}/posts/${dto.postId}/comments`, dto);
    },

    async deleteComment(commentId: string): Promise<void> {
        return apiClient.delete<void>(`${BASE_URL}/comments/${commentId}`);
    }
};

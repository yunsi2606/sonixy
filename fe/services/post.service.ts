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
        const mediaItems: { type: 'image' | 'video', objectKey: string }[] = [];

        if (files && files.length > 0) {
            // Upload each file sequentially (or parallel if preferred, but sequential is safer for rate limits/order)
            // Using parallel here for speed as MinIO can handle it
            const uploadPromises = files.map(async (file) => {
                // 1. Get Presigned URL
                const { uploadUrl, objectKey } = await apiClient.post<{ uploadUrl: string, objectKey: string }>(
                    '/api/posts/presigned-url',
                    { fileName: file.name, contentType: file.type }
                );

                // 2. Upload to MinIO directly
                await fetch(uploadUrl, {
                    method: 'PUT',
                    body: file,
                    headers: {
                        'Content-Type': file.type
                    }
                });

                return {
                    type: file.type.startsWith('video') ? 'video' as const : 'image' as const,
                    objectKey
                };
            });

            const results = await Promise.all(uploadPromises);
            mediaItems.push(...results);
        }

        // 3. Create Post with Media References
        return apiClient.post<Post>('/api/posts', {
            content,
            visibility,
            media: mediaItems
        });
    },

    async toggleLike(postId: string): Promise<{ success: boolean }> {
        return apiClient.post<{ success: boolean }>(`/api/posts/${postId}/like`, {});
    }
};

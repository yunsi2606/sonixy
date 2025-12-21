import { apiClient } from '@/lib/api';
import { authService } from './auth.service';
import type { User } from '@/types/api';

const USER_BASE = '/api/users';

export interface UpdateUserDto {
    firstName?: string;
    lastName?: string;
    bio?: string;
    avatarUrl?: string;
}

export const userService = {
    async getUser(id: string): Promise<User> {
        const token = authService.getAccessToken();
        return apiClient.get<User>(`${USER_BASE}/${id}`, token || undefined);
    },

    async getCurrentUser(): Promise<User> {
        return apiClient.get<User>(`${USER_BASE}/me`);
    },

    async checkUsernameAvailability(username: string): Promise<boolean> {
        const response = await apiClient.get<{ available: boolean }>(`${USER_BASE}/check-username?username=${encodeURIComponent(username)}`);
        return response.available;
    },

    async updateUser(id: string, data: UpdateUserDto): Promise<User> {
        const token = authService.getAccessToken();
        return apiClient.patch<User>(`${USER_BASE}/${id}`, data, token || undefined);
    },

    async uploadAvatar(file: File): Promise<string> {
        const { uploadUrl, publicUrl } = await apiClient.post<{ uploadUrl: string, objectKey: string, publicUrl: string }>(
            `${USER_BASE}/presigned-url`,
            { fileName: file.name, contentType: file.type }
        );

        await fetch(uploadUrl, {
            method: 'PUT',
            body: file,
            headers: {
                'Content-Type': file.type
            }
        });

        return publicUrl;
    },
};

import { apiClient } from '@/lib/api';
import { authService } from './auth.service';
import type { User } from '@/types/api';

const USER_BASE = '/api/users';

export interface UpdateUserDto {
    displayName?: string;
    bio?: string;
    avatarUrl?: string;
}

export const userService = {
    async getUser(id: string): Promise<User> {
        const token = authService.getAccessToken();
        return apiClient.get<User>(`${USER_BASE}/${id}`, token || undefined);
    },

    async getCurrentUser(): Promise<User> {
        const userId = authService.getUserId();
        if (!userId) throw new Error('Not authenticated');

        const token = authService.getAccessToken();
        return apiClient.get<User>(`${USER_BASE}/${userId}`, token || undefined);
    },

    async updateUser(id: string, data: UpdateUserDto): Promise<User> {
        const token = authService.getAccessToken();
        return apiClient.patch<User>(`${USER_BASE}/${id}`, data, token || undefined);
    },
};

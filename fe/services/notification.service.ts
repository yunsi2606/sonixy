import { apiClient } from '@/lib/api';

export interface Notification {
    id: string;
    actorId: string;
    actorName: string;
    actorAvatar: string;
    entityId: string;
    entityType: string; // Post, Comment
    action: string; // Like, Comment, Reply
    isRead: boolean;
    createdAt: string;
}

export interface NotificationResponse {
    items: Notification[];
    unreadCount: number;
    hasMore: boolean;
}

export const NotificationService = {
    getNotifications: async (pageIndex: number = 1, pageSize: number = 20): Promise<NotificationResponse> => {
        const response = await apiClient.get<NotificationResponse>(`/notifications?pageIndex=${pageIndex}&pageSize=${pageSize}`);
        return response;
    },

    markAsRead: async (id: string): Promise<void> => {
        await apiClient.put(`/notifications/${id}/read`, {});
    },

    markAllAsRead: async (): Promise<void> => {
        await apiClient.put(`/notifications/read-all`, {});
    }
};

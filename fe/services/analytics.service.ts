import { apiClient } from '@/lib/api';

export enum EventType {
    VIEW = 'View',
    CLICK = 'Click',
    LIKE = 'Like',
    SHARE = 'Share',
    COMMENT = 'Comment',
    SCROLL = 'Scroll'
}

export interface AnalyticsEvent {
    eventType: EventType;
    targetId: string;
    targetType: string;
    metadata?: Record<string, any>;
}

const ANALYTICS_BASE = '/api/analytics';

export const analyticsService = {
    async trackEvent(event: AnalyticsEvent): Promise<void> {
        // Fire and forget (don't await strictly in UI if not needed)
        try {
            await apiClient.post(`${ANALYTICS_BASE}/events`, event);
        } catch (error) {
            console.error('Failed to track event:', error);
        }
    },

    async trackView(postId: string): Promise<void> {
        return this.trackEvent({
            eventType: EventType.VIEW,
            targetId: postId,
            targetType: 'Post'
        });
    },

    async trackLike(postId: string): Promise<void> {
        return this.trackEvent({
            eventType: EventType.LIKE,
            targetId: postId,
            targetType: 'Post'
        });
    }
};

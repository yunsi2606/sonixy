'use client';

import React, { useEffect } from 'react';
import { useNotification } from '@/contexts/NotificationContext';
import { NotificationItem } from '@/components/notifications/NotificationItem';
import { NotificationListSkeleton } from '@/components/skeletons/NotificationSkeleton';

export default function NotificationsPage() {
    const { notifications, fetchNotifications, markAsRead, markAllAsRead, isLoading } = useNotification();

    useEffect(() => {
        fetchNotifications(1);
    }, [fetchNotifications]);

    const handleNotificationClick = (id: string) => {
        markAsRead(id);
        // Navigate or open modal logic here
    };

    return (
        <div className="w-full">
            <div className="sticky top-0 z-10 glass-base border-b border-[var(--glass-border)] p-4 flex items-center justify-between">
                <h1 className="text-xl font-bold text-white">Notifications</h1>
                <button
                    onClick={() => markAllAsRead()}
                    className="text-xs font-bold text-[var(--color-primary)] hover:text-white transition-colors"
                >
                    Mark all as read
                </button>
            </div>

            <div className="min-h-[50vh]">
                {isLoading && notifications?.length === 0 ? (
                    <NotificationListSkeleton />
                ) : notifications?.length === 0 ? (
                    <div className="flex flex-col items-center justify-center py-20 text-[var(--color-text-muted)]">
                        <span className="text-4xl mb-4">🔕</span>
                        <p>No notifications yet</p>
                    </div>
                ) : (
                    <div>
                        {notifications?.map(notification => (
                            <NotificationItem
                                key={notification.id}
                                notification={notification}
                                onClick={() => handleNotificationClick(notification.id)}
                            />
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}

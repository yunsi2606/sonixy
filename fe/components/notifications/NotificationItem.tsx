import React from 'react';
import { Notification } from '@/services/notification.service';
import Link from 'next/link';
import { formatDistanceToNow } from 'date-fns';

interface NotificationItemProps {
    notification: Notification;
    onClick: () => void;
}

export const NotificationItem: React.FC<NotificationItemProps> = ({ notification, onClick }) => {
    // Generate content based on Action/EntityType
    const getMessage = () => {
        switch (notification.action) {
            case 'Like':
                return `liked your ${notification.entityType.toLowerCase()}.`;
            case 'Comment':
                return `commented on your ${notification.entityType.toLowerCase()}.`;
            case 'Reply':
                return `replied to your comment.`;
            default:
                return 'interacted with you.';
        }
    };

    // Deep link url
    const href = `/feed`; // Ideally /post/{id} but feed for now as post detail page might not exist or be different? 
    // Assuming simple redirection to feed or specific post logic if implemented.
    // User requested "Notification for existing functionality".
    // I can link to `/u/[username]/status/[postId]` if it exists, or just `/feed`.

    return (
        <div
            onClick={onClick}
            className={`p-4 border-b border-[var(--glass-border)] cursor-pointer transition-colors ${notification.isRead ? 'bg-transparent hover:bg-white/5' : 'bg-[var(--color-primary)]/10 hover:bg-[var(--color-primary)]/20'
                }`}
        >
            <div className="flex items-start gap-4">
                <Link href={`/u/${notification.actorName}`} onClick={(e) => e.stopPropagation()}>
                    <img
                        src={notification.actorAvatar || "https://api.dicebear.com/9.x/avataaars/svg?seed=" + notification.actorId}
                        alt={notification.actorName}
                        className="w-10 h-10 rounded-full object-cover border border-[var(--glass-border)]"
                    />
                </Link>
                <div className="flex-1 min-w-0">
                    <p className="text-sm text-[var(--color-text-primary)] leading-snug">
                        <Link href={`/u/${notification.actorName}`} onClick={(e) => e.stopPropagation()} className="font-bold hover:underline">
                            {notification.actorName}
                        </Link>
                        {' '}
                        <span className="text-[var(--color-text-secondary)]">{getMessage()}</span>
                    </p>
                    <p className="text-xs text-[var(--color-text-muted)] mt-1">
                        {formatDistanceToNow(new Date(notification.createdAt), { addSuffix: true })}
                    </p>
                </div>
                {!notification.isRead && (
                    <div className="w-2 h-2 rounded-full bg-[var(--color-primary)] mt-2" />
                )}
            </div>
        </div>
    );
};

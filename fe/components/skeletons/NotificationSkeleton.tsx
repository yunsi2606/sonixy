import { Skeleton } from '@/components/ui/Skeleton';

export function NotificationSkeleton() {
    return (
        <div className="flex gap-3 p-4 border-b border-[var(--glass-border)] animate-pulse">
            {/* Avatar */}
            <Skeleton variant="circle" className="w-10 h-10 flex-shrink-0" />

            <div className="flex-1 space-y-2">
                {/* Content */}
                <div className="flex flex-col gap-1.5">
                    <Skeleton variant="text" className="w-[85%] h-4" />
                    <Skeleton variant="text" className="w-20 h-3 opacity-50" />
                </div>
            </div>

            {/* Unread Indicator */}
            <Skeleton variant="circle" className="w-3 h-3 flex-shrink-0 mt-2" />
        </div>
    );
}

export function NotificationListSkeleton() {
    return (
        <div className="flex flex-col">
            <NotificationSkeleton />
            <NotificationSkeleton />
            <NotificationSkeleton />
            <NotificationSkeleton />
            <NotificationSkeleton />
            <NotificationSkeleton />
        </div>
    );
}

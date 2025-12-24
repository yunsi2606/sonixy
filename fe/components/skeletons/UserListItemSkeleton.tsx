import { Skeleton } from '@/components/ui/Skeleton';

export function UserListItemSkeleton() {
    return (
        <div className="flex items-center gap-3 p-2">
            {/* Avatar */}
            <Skeleton variant="circle" className="w-10 h-10 flex-shrink-0" />

            {/* User Info */}
            <div className="flex-1 min-w-0 space-y-2">
                <Skeleton variant="text" className="w-32 h-4" />
                <Skeleton variant="text" className="w-24 h-3 opacity-60" />
            </div>

            {/* Action Button */}
            <Skeleton variant="default" className="w-20 h-8 rounded-full opacity-40" />
        </div>
    );
}

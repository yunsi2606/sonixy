import { Skeleton } from '@/components/ui/Skeleton';

export function CommentSkeleton() {
    return (
        <div className="flex gap-3 p-3 rounded-xl bg-white/5 animate-pulse mb-3">
            {/* Avatar */}
            <Skeleton variant="circle" className="w-8 h-8 flex-shrink-0" />

            <div className="flex-1 space-y-2">
                {/* Header: Name + Date */}
                <div className="flex items-center gap-2">
                    <Skeleton variant="text" className="w-24 h-3" />
                    <Skeleton variant="text" className="w-16 h-3 opacity-50" />
                </div>

                {/* Content */}
                <Skeleton variant="text" className="w-full h-3" />
                <Skeleton variant="text" className="w-[80%] h-3" />

                {/* Actions */}
                <div className="flex gap-4 pt-1">
                    <Skeleton variant="text" className="w-8 h-3 opacity-40" />
                    <Skeleton variant="text" className="w-8 h-3 opacity-40" />
                </div>
            </div>
        </div>
    );
}

export function CommentThreadSkeleton() {
    return (
        <div className="flex flex-col gap-4">
            <CommentSkeleton />
            <CommentSkeleton />
            <div className="ml-10 border-l-2 border-white/10 pl-4">
                <CommentSkeleton />
            </div>
            <CommentSkeleton />
        </div>
    );
}

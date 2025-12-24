import { Skeleton } from '@/components/ui/Skeleton';

export function PostSkeleton() {
    return (
        <div className="glass-base rounded-2xl p-6 mb-8 animate-fade-in">
            {/* Header */}
            <div className="flex items-center gap-4 mb-4">
                <Skeleton variant="circle" className="w-12 h-12" />
                <div className="flex-1 space-y-2">
                    <Skeleton variant="text" className="w-32 h-4" />
                    <Skeleton variant="text" className="w-24 h-3 opacity-60" />
                </div>
                <Skeleton variant="circle" className="w-8 h-8 opacity-40" />
            </div>

            {/* Content Text */}
            <div className="space-y-3 mb-6">
                <Skeleton variant="text" className="w-full h-4" />
                <Skeleton variant="text" className="w-[90%] h-4" />
                <Skeleton variant="text" className="w-[80%] h-4" />
            </div>

            {/* Content Image (Optional placeholder) */}
            <Skeleton variant="default" className="w-full h-64 rounded-xl mb-6 opacity-40" />

            {/* Actions */}
            <div className="flex justify-between border-t border-white/10 pt-4 opacity-50">
                <div className="flex gap-6">
                    <Skeleton variant="text" className="w-16 h-8 rounded-full" />
                    <Skeleton variant="text" className="w-16 h-8 rounded-full" />
                </div>
                <Skeleton variant="text" className="w-8 h-8 rounded-full" />
            </div>
        </div>
    );
}

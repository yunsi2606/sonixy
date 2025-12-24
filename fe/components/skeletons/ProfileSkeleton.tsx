import { Skeleton } from '@/components/ui/Skeleton';
import { PostSkeleton } from './PostSkeleton';
import { ProfileStatsSkeleton } from './ProfileStatsSkeleton';

export function ProfileSkeleton() {
    return (
        <div className="pb-8">
            {/* Header Skeleton */}
            <div className="glass rounded-2xl overflow-hidden mb-8 animate-fade-in relative">
                {/* Cover Image */}
                <Skeleton variant="default" className="h-48 w-full rounded-none opacity-60" />

                <div className="px-8 pb-8">
                    <div className="flex justify-between items-end -mt-12 mb-6">
                        {/* Avatar */}
                        <div className="relative">
                            <div className="w-32 h-32 rounded-full border-4 border-[var(--color-bg-primary)] overflow-hidden bg-[var(--color-surface)]">
                                <Skeleton variant="circle" className="w-full h-full" />
                            </div>
                        </div>

                        {/* Action Button */}
                        <div className="mb-4">
                            <Skeleton variant="default" className="w-32 h-10 rounded-xl opacity-40" />
                        </div>
                    </div>

                    {/* Info */}
                    <div className="mb-6 space-y-2">
                        <Skeleton variant="text" className="w-48 h-8 mb-2" />
                        <Skeleton variant="text" className="w-32 h-4 opacity-60" />
                        <Skeleton variant="text" className="w-full max-w-lg h-4 opacity-80 pt-2" />
                        <Skeleton variant="text" className="w-full max-w-md h-4 opacity-80" />
                    </div>

                    {/* Stats */}
                    <ProfileStatsSkeleton />
                </div>
            </div>

            {/* Posts Section Skeleton */}
            <div>
                <Skeleton variant="text" className="w-32 h-8 mb-6" />
                <div className="space-y-4">
                    <PostSkeleton />
                    <PostSkeleton />
                </div>
            </div>
        </div>
    );
}

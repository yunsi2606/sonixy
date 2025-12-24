import { Skeleton } from '@/components/ui/Skeleton';

export function ProfileStatsSkeleton() {
    return (
        <div className="flex gap-8 border-t border-[var(--color-border)] pt-6">
            {[1, 2, 3].map((i) => (
                <div key={i} className="text-center w-20">
                    <Skeleton variant="text" className="w-8 h-6 mx-auto mb-1" />
                    <div className="text-sm text-[var(--color-text-secondary)]">
                        <Skeleton variant="text" className="w-12 h-3 mx-auto opacity-50" />
                    </div>
                </div>
            ))}
        </div>
    );
}

import { cn } from '@/lib/utils';
import React from 'react';

interface SkeletonProps extends React.HTMLAttributes<HTMLDivElement> {
    variant?: 'default' | 'circle' | 'text';
}

export function Skeleton({ className, variant = 'default', ...props }: SkeletonProps) {
    return (
        <div
            className={cn(
                "animate-pulse bg-white/10",
                {
                    'rounded-lg': variant === 'default',
                    'rounded-full': variant === 'circle',
                    'rounded h-4': variant === 'text',
                },
                className
            )}
            {...props}
        />
    );
}

import React from 'react';

interface AvatarProps {
    src?: string | null;
    alt?: string;
    username?: string; // Used for initials if alt is missing
    size?: 'sm' | 'md' | 'lg' | 'xl';
    hasRing?: boolean;
    className?: string; // For custom sizing or positioning if needed
}

export const Avatar: React.FC<AvatarProps> = ({
    src,
    alt = '',
    username = '',
    size = 'md',
    hasRing = true,
    className = ''
}) => {
    // Determine sizes
    const sizeClasses = {
        sm: 'w-8 h-8',
        md: 'w-10 h-10',
        lg: 'w-12 h-12',
        xl: 'w-16 h-16'
    };

    const containerSize = sizeClasses[size];

    // Calculate initials
    const displayName = alt || username || 'User';
    const initials = displayName
        .split(' ')
        .map(n => n[0])
        .join('')
        .slice(0, 2)
        .toUpperCase();

    // The ring wrapper
    if (hasRing) {
        return (
            <div className={`relative rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] p-[2px] ${containerSize} ${className}`}>
                <div className="w-full h-full rounded-full bg-[var(--color-bg-deep)] relative overflow-hidden flex items-center justify-center">
                    {src ? (
                        <img src={src} alt={alt} className="w-full h-full object-cover" />
                    ) : (
                        <div className="w-full h-full bg-[var(--color-surface)] flex items-center justify-center">
                            <span className="text-xs font-bold text-[var(--color-text-muted)]">{initials}</span>
                        </div>
                    )}
                </div>
            </div>
        );
    }

    // No ring
    return (
        <div className={`relative rounded-full bg-[var(--color-surface)] overflow-hidden flex items-center justify-center ${containerSize} ${className}`}>
            {src ? (
                <img src={src} alt={alt} className="w-full h-full object-cover" />
            ) : (
                <span className="text-xs font-bold text-[var(--color-text-muted)]">{initials}</span>
            )}
        </div>
    );
};

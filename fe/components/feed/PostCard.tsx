import type { Post } from '@/types/api';
import React from 'react';

interface PostCardProps {
    post: Post;
    variant?: 'default' | 'hero' | 'compact';
    onLike?: (id: string) => void;
    onComment?: (id: string) => void;
}

export function PostCard({ post, variant = 'default', onLike, onComment }: PostCardProps) {
    // Format date relative or locale
    // In real app, consider using date-fns formatDistanceToNow
    const date = new Date(post.createdAt);
    const timeDisplay = date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });

    // --- Variant Styles ---
    const containerClasses = {
        default: "card-social group mb-6 hover:translate-y-[-4px] transition-all duration-300",
        hero: "group mb-8 relative overflow-hidden rounded-3xl p-8 border border-[var(--glass-border-light)] shadow-[var(--shadow-lift)] hover:scale-[1.01] transition-all duration-500",
        compact: "glass-base rounded-xl p-4 mb-3 border border-[var(--glass-border)] hover:bg-white/5 transition-colors"
    };

    const textClasses = {
        default: "text-[var(--color-text-primary)] text-[15px] leading-relaxed whitespace-pre-wrap",
        hero: "text-white text-2xl md:text-3xl font-bold leading-tight mb-6 bg-gradient-to-br from-white to-white/80 bg-clip-text text-transparent",
        compact: "text-[var(--color-text-primary)] text-sm leading-normal"
    };

    // Render Hero Card Background
    if (variant === 'hero') {
        return (
            <article className={containerClasses.hero}>
                <div className="absolute inset-0 bg-gradient-to-br from-[var(--color-primary)]/10 via-purple-900/20 to-[var(--color-bg-deep)] z-0" />
                <div className="absolute top-0 right-0 p-[200px] bg-[var(--color-secondary)]/20 blur-[100px] rounded-full pointer-events-none" />

                <div className="relative z-10">
                    <div className="flex items-center gap-3 mb-6">
                        <div className="px-3 py-1 rounded-full bg-white/10 border border-white/20 text-xs font-bold text-white uppercase tracking-wider">
                            Featured
                        </div>
                        <div className="text-white/60 text-sm">{timeDisplay}</div>
                    </div>

                    <p className={textClasses.hero}>
                        {post.content}
                    </p>

                    <div className="flex items-center gap-4 mt-8">
                        <div className="flex items-center gap-3">
                            <div className="w-10 h-10 rounded-full bg-white/20 ring-2 ring-white/10" />
                            <div className="font-semibold text-white">Author Name</div>
                        </div>
                        <div className="ml-auto">
                            <button className="btn-primary shadow-lg scale-90 hover:scale-100 transition-transform">
                                Read Article ➔
                            </button>
                        </div>
                    </div>
                </div>
            </article>
        );
    }

    // Default & Compact Render
    return (
        <article className={containerClasses[variant]}>
            {/* Header */}
            <div className={`flex items-center gap-3 ${variant === 'compact' ? 'mb-2' : 'mb-4'}`}>
                <div className="relative cursor-pointer avatar-ring">
                    <div className={`rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] p-[2px] ${variant === 'compact' ? 'w-8 h-8' : 'w-10 h-10'}`}>
                        <div className="w-full h-full rounded-full bg-[var(--color-bg-deep)] relative overflow-hidden">
                            {/* Placeholder for Avatar */}
                            <div className="w-full h-full flex items-center justify-center text-sm font-bold bg-white/5 text-[var(--color-text-secondary)]">U</div>
                        </div>
                    </div>
                </div>

                <div className="flex flex-col">
                    <div className="flex items-center gap-2">
                        <span className="font-semibold text-[var(--color-text-primary)] hover:text-[var(--color-primary)] cursor-pointer transition-colors text-sm">
                            Username
                        </span>
                        {variant !== 'compact' && <span className="text-xs text-[var(--color-text-muted)]">• {timeDisplay}</span>}
                    </div>
                </div>

                <button className="ml-auto p-2 text-[var(--color-text-muted)] hover:text-white rounded-full hover:bg-white/5 transition-colors">
                    <span className="text-lg">⋯</span>
                </button>
            </div>

            {/* Content */}
            <div className={variant === 'compact' ? 'mb-2' : 'mb-4'}>
                <p className={textClasses[variant]}>
                    {post.content}
                </p>
            </div>

            {/* Actions */}
            <div className={`flex items-center ${variant === 'compact' ? 'gap-6 pt-1' : 'pt-2 border-t border-[var(--glass-border)]'}`}>
                <ActionButton
                    icon={post.isLiked ? "❤️" : "🤍"}
                    count={post.likeCount}
                    onClick={() => onLike?.(post.id)}
                    activeColor={post.isLiked ? "text-red-500 scale-110" : "text-pink-500"}
                    isActive={post.isLiked}
                />
                <ActionButton icon="💬" count="Comment" onClick={() => onComment?.(post.id)} activeColor="text-[var(--color-primary)]" />
                <ActionButton icon="📤" count="Share" activeColor="text-[var(--color-secondary)]" className="ml-auto" />
            </div>
        </article>
    );
}

// Sub-component for buttons to reduce repetition
function ActionButton({ icon, count, onClick, activeColor, className = "", isActive }: any) {
    return (
        <button
            onClick={onClick}
            className={`group/btn flex items-center gap-2 px-2 py-1.5 rounded-lg hover:bg-white/5 transition-all outline-none ${className}`}
        >
            <span className={`text-lg group-hover/btn:scale-110 transition-transform duration-200 ${isActive ? '' : 'grayscale group-hover/btn:grayscale-0'} ${activeColor}`}>
                {icon}
            </span>
            {count && (
                <span className={`text-xs font-medium ${isActive ? 'text-white' : 'text-[var(--color-text-muted)] group-hover/btn:text-white'}`}>
                    {count}
                </span>
            )}
        </button>
    );
}

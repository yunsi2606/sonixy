import type { Post } from '@/types/api';

interface PostCardProps {
    post: Post;
}

export function PostCard({ post }: PostCardProps) {
    return (
        <article className="card card-hover group relative overflow-hidden">
            {/* Gradient Border Effect on Hover */}
            <div className="absolute inset-0 opacity-0 group-hover:opacity-100 transition-opacity duration-300 pointer-events-none">
                <div className="absolute inset-0 bg-gradient-to-r from-primary/20 via-secondary/20 to-accent/20 blur-xl" />
            </div>

            <div className="relative flex items-start gap-4">
                {/* Avatar with Gradient */}
                <div className="relative flex-shrink-0">
                    <div className="w-12 h-12 rounded-full bg-gradient-to-br from-primary via-secondary to-accent p-0.5">
                        <div className="w-full h-full rounded-full bg-bg-secondary flex items-center justify-center text-xl">
                            👤
                        </div>
                    </div>
                    <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-gradient-to-br from-primary to-secondary rounded-full border-2 border-bg-primary" />
                </div>

                <div className="flex-1 min-w-0">
                    {/* Header */}
                    <div className="flex items-center gap-3 mb-3">
                        <span className="font-bold text-text-primary text-lg">User</span>
                        <span className="px-2 py-0.5 bg-primary/20 text-primary text-xs font-semibold rounded-full">
                            Pro
                        </span>
                        <span className="text-sm text-text-muted">•</span>
                        <span className="text-sm text-text-muted">
                            {new Date(post.createdAt).toLocaleDateString('en-US', {
                                month: 'short',
                                day: 'numeric',
                                year: 'numeric'
                            })}
                        </span>
                    </div>

                    {/* Content */}
                    <p className="text-text-secondary text-base sm:text-lg leading-relaxed mb-6 whitespace-pre-wrap">
                        {post.content}
                    </p>

                    {/* Actions Bar */}
                    <div className="flex items-center gap-6">
                        <button className="group/like flex items-center gap-2 text-text-muted hover:text-accent transition-all duration-200">
                            <span className="text-xl group-hover/like:scale-125 transition-transform duration-200">
                                ❤️
                            </span>
                            <span className="font-semibold">
                                {post.likeCount > 0 ? post.likeCount : 'Like'}
                            </span>
                        </button>

                        <button className="group/comment flex items-center gap-2 text-text-muted hover:text-primary transition-all duration-200">
                            <span className="text-xl group-hover/comment:scale-125 transition-transform duration-200">
                                💬
                            </span>
                            <span className="font-semibold">Comment</span>
                        </button>

                        <button className="group/share flex items-center gap-2 text-text-muted hover:text-secondary transition-all duration-200 ml-auto">
                            <span className="text-xl group-hover/share:scale-125 transition-transform duration-200">
                                📤
                            </span>
                            <span className="font-semibold">Share</span>
                        </button>
                    </div>
                </div>
            </div>
        </article>
    );
}

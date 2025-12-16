import React from 'react';

// Define interface locally if not in api.ts
export interface Comment {
    id: string;
    author: {
        name: string;
        avatar?: string;
    };
    content: string;
    createdAt: string;
    replies?: Comment[];
}

interface CommentThreadProps {
    comments: Comment[];
    depth?: number;
}

export function CommentThread({ comments, depth = 0 }: CommentThreadProps) {
    if (!comments || comments.length === 0) return null;

    return (
        <div className={`flex flex-col gap-4 ${depth > 0 ? 'ml-6 border-l-2 border-[var(--glass-border)] pl-4' : ''}`}>
            {comments.map((comment) => (
                <CommentItem key={comment.id} comment={comment} depth={depth} />
            ))}
        </div>
    );
}

function CommentItem({ comment, depth }: { comment: Comment; depth: number }) {
    return (
        <div className="group relative">
            <div className={`p-3 rounded-xl transition-all duration-300 ${depth === 0 ? 'bg-white/5 hover:bg-white/10' : 'bg-transparent hover:bg-white/5'
                }`}>
                <div className="flex gap-3">
                    {/* Avatar */}
                    <div className="w-8 h-8 rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] p-[1px] flex-shrink-0">
                        <div className="w-full h-full rounded-full bg-[var(--color-bg-deep)]" />
                    </div>

                    <div className="flex-1">
                        <div className="flex items-center gap-2 mb-1">
                            <span className="font-semibold text-sm text-white/90 hover:text-[var(--color-primary)] transition-colors cursor-pointer">
                                {comment.author.name}
                            </span>
                            <span className="text-xs text-white/40">
                                {new Date(comment.createdAt).toLocaleDateString()}
                            </span>
                        </div>

                        <p className="text-sm text-white/80 leading-relaxed">
                            {comment.content}
                        </p>

                        {/* Actions */}
                        <div className="flex items-center gap-4 mt-2">
                            <button className="text-xs font-medium text-white/40 hover:text-white transition-colors">
                                Reply
                            </button>
                            <button className="text-xs font-medium text-white/40 hover:text-pink-400 transition-colors">
                                Like
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            {/* Nested Replies */}
            {comment.replies && comment.replies.length > 0 && (
                <div className="mt-3">
                    <CommentThread comments={comment.replies} depth={depth + 1} />
                </div>
            )}
        </div>
    );
}

import React from 'react';
import type { Comment } from '@/types/comment';

interface CommentItemProps {
    comment: Comment;
    onReply: (comment: Comment) => void;
    isReply?: boolean;
}

export function CommentItem({ comment, onReply, isReply = false }: CommentItemProps) {
    const timeDisplay = new Date(comment.createdAt).toLocaleDateString(undefined, {
        month: 'short',
        day: 'numeric'
    });

    return (
        <div className={`flex gap-2 ${isReply ? 'ml-12 mt-2' : 'mt-4'}`}>
            {/* Avatar */}
            <div className="flex-shrink-0">
                <div className={`${isReply ? 'w-6 h-6' : 'w-8 h-8'} rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] p-[1px]`}>
                    <div className="w-full h-full rounded-full bg-[var(--color-bg-deep)] overflow-hidden">
                        {comment.author.avatarUrl ? (
                            <img src={comment.author.avatarUrl} alt={comment.author.username} className="w-full h-full object-cover" />
                        ) : (
                            <div className="w-full h-full flex items-center justify-center text-[10px] font-bold bg-white/10 text-white">
                                {comment.author.username[0].toUpperCase()}
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* Content Body */}
            <div className="flex-1">
                <div className="bg-white/5 rounded-2xl px-3 py-2 inline-block min-w-[150px]">
                    <div className="font-semibold text-sm text-white/90">
                        {comment.author.username}
                    </div>
                    <div className="text-[13px] text-white/80 leading-relaxed break-words">
                        {comment.replyTo && (
                            <span className="font-medium text-[var(--color-primary)] hover:underline cursor-pointer mr-1">
                                @{comment.replyTo.username}
                            </span>
                        )}
                        {comment.content}
                    </div>
                </div>

                {/* Actions Line */}
                <div className="flex items-center gap-3 mt-1 ml-1">
                    <span className="text-xs text-white/40">{timeDisplay}</span>
                    <button className="text-xs font-semibold text-white/60 hover:text-white transition-colors">
                        Like
                    </button>
                    <button
                        onClick={() => onReply(comment)}
                        className="text-xs font-semibold text-white/60 hover:text-white transition-colors"
                    >
                        Reply
                    </button>
                    {comment.likes > 0 && (
                        <div className="flex items-center gap-1 text-xs text-pink-500">
                            ❤️ {comment.likes}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

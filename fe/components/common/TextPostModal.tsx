import React, { useEffect } from 'react';
import { createPortal } from 'react-dom';
import type { Post } from '@/types/api';
import { CommentSection } from '@/components/comments/CommentSection';
import { Avatar } from '@/components/ui/Avatar';
import { X, Heart, MessageCircle, Share2, MoreHorizontal } from 'lucide-react';
import Link from 'next/link';

interface TextPostModalProps {
    isOpen: boolean;
    onClose: () => void;
    post: Post;
    onLike?: (id: string) => void;
}

export function TextPostModal({ isOpen, onClose, post, onLike }: TextPostModalProps) {
    const [mounted, setMounted] = React.useState(false);

    useEffect(() => {
        setMounted(true);
        return () => setMounted(false);
    }, []);

    useEffect(() => {
        if (isOpen) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = 'unset';
            // Also close it when unmounting
        }
        return () => {
            document.body.style.overflow = 'unset';
        };
    }, [isOpen]);

    if (!mounted || !isOpen) return null;

    const date = new Date(post.createdAt);
    const timeDisplay = date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });

    return createPortal(
        <div
            className="fixed inset-0 z-[9999] bg-black/80 backdrop-blur-sm flex items-center justify-center p-4 sm:p-6 transition-opacity"
            onClick={onClose}
        >
            <div
                className="bg-[var(--color-bg-deep)] border border-[var(--glass-border)] rounded-2xl w-full max-w-2xl h-[90vh] min-h-[500px] flex flex-col shadow-2xl relative overflow-hidden animate-in fade-in zoom-in-95 duration-200"
                onClick={e => e.stopPropagation()}
            >
                {/* Header */}
                <div className="flex items-center justify-between p-4 border-b border-[var(--glass-border)] bg-black/20 shrink-0">
                    <h2 className="font-bold text-white text-lg flex-1 text-center truncate">Post by {post.authorDisplayName}</h2>
                    <button onClick={onClose} className="p-2 rounded-full bg-white/5 hover:bg-white/10 text-[var(--color-text-muted)] hover:text-white transition-colors absolute right-4">
                        <X size={20} />
                    </button>
                </div>

                {/* Body - Post Content + Comments */}
                <div className="flex-1 flex flex-col min-h-0 overflow-hidden">
                    {/* Post Content */}
                    <div className="p-5 border-b border-[var(--glass-border)] shrink-0 max-h-[40vh] overflow-y-auto custom-scrollbar">
                        <div className="flex items-center gap-3 mb-4">
                            <Link href={`/u/${post.authorUsername}`} onClick={onClose}>
                                <Avatar src={post.authorAvatarUrl} username={post.authorDisplayName} size="md" />
                            </Link>
                            <div>
                                <Link href={`/u/${post.authorUsername}`} onClick={onClose} className="font-semibold text-white hover:text-[var(--color-primary)] transition-colors">
                                    {post.authorDisplayName}
                                </Link>
                                <div className="text-xs text-[var(--color-text-muted)]">@{post.authorUsername} • {timeDisplay}</div>
                            </div>
                            <button className="ml-auto p-2 text-[var(--color-text-muted)] hover:text-white rounded-full hover:bg-white/5 transition-colors">
                                <MoreHorizontal size={20} />
                            </button>
                        </div>

                        <p className="text-[var(--color-text-primary)] text-[16px] leading-relaxed whitespace-pre-wrap mb-4 font-medium">
                            {post.content.split(/(#\w+)/g).map((chunk, i) => {
                                if (chunk.startsWith('#')) {
                                    return (
                                        <Link
                                            key={i}
                                            href={`/search?q=${encodeURIComponent(chunk)}`}
                                            className="text-[var(--color-primary)] hover:text-[var(--color-secondary)] transition-colors inline-block"
                                            onClick={onClose}
                                        >
                                            {chunk}
                                        </Link>
                                    );
                                }
                                return chunk;
                            })}
                        </p>

                        {/* Actions */}
                        <div className="flex items-center pt-2 border-t border-[var(--glass-border)]/50">
                            <button
                                onClick={() => onLike?.(post.id)}
                                className="group flex items-center gap-2 px-2 py-1.5 rounded-lg hover:bg-white/5 transition-all outline-none"
                            >
                                <span className={`text-lg transition-transform duration-200 ${post.isLiked ? 'text-red-500 scale-110' : 'text-pink-500 grayscale group-hover:grayscale-0'}`}>
                                    <Heart size={20} className={post.isLiked ? "fill-current" : ""} />
                                </span>
                                <span className={`text-xs font-medium ${post.isLiked ? 'text-white' : 'text-[var(--color-text-muted)] group-hover:text-white'}`}>
                                    {post.likeCount}
                                </span>
                            </button>
                            <button className="group flex items-center gap-2 px-2 py-1.5 rounded-lg hover:bg-white/5 transition-all outline-none">
                                <span className="text-lg text-[var(--color-primary)] grayscale group-hover:grayscale-0">
                                    <MessageCircle size={20} />
                                </span>
                                <span className="text-xs font-medium text-[var(--color-text-muted)] group-hover:text-white">
                                    Comment
                                </span>
                            </button>
                            <button className="group flex items-center gap-2 px-2 py-1.5 rounded-lg hover:bg-white/5 transition-all outline-none ml-auto">
                                <span className="text-lg text-[var(--color-secondary)] grayscale group-hover:grayscale-0">
                                    <Share2 size={20} />
                                </span>
                                <span className="text-xs font-medium text-[var(--color-text-muted)] group-hover:text-white">
                                    Share
                                </span>
                            </button>
                        </div>
                    </div>

                    {/* Comments Section */}
                    <div className="flex-1 min-h-0 flex flex-col">
                        <CommentSection post={post} />
                    </div>
                </div>
            </div>
        </div>,
        document.body
    );
}

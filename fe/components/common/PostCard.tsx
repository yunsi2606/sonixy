import Link from 'next/link';
import { useState, useEffect } from 'react';
import { userService } from '@/services/user.service';
import type { Post, User } from '@/types/api';

interface PostCardProps {
    post: Post;
}

export function PostCard({ post }: PostCardProps) {
    const authorName = post.authorName || 'Unknown User';
    const authorAvatar = post.authorAvatarUrl;

    // Fallback initials from name if fetching failed or not available logic 
    // Simplified logic: take first letter of display name parts
    const authorInitials = authorName.split(' ').map(n => n[0]).join('').slice(0, 2).toUpperCase();

    return (
        <article className="card card-hover group relative overflow-hidden">
            {/* Gradient Border Effect on Hover */}
            <div className="absolute inset-0 opacity-0 group-hover:opacity-100 transition-opacity duration-300 pointer-events-none">
                <div className="absolute inset-0 bg-gradient-to-r from-primary/20 via-secondary/20 to-accent/20 blur-xl" />
            </div>

            <div className="relative flex items-start gap-4">
                {/* Avatar with Gradient */}
                <div className="relative flex-shrink-0">
                    <Link href={`/users/${post.authorId}`} className="relative z-10 block">
                        <div className="w-12 h-12 rounded-full bg-gradient-to-br from-primary via-secondary to-accent p-0.5 cursor-pointer hover:scale-105 transition-transform">
                            <div className="w-full h-full rounded-full bg-bg-secondary flex items-center justify-center overflow-hidden">
                                {authorAvatar ? (
                                    <img src={authorAvatar} alt={authorName} className="w-full h-full object-cover" />
                                ) : (
                                    <span className="text-lg font-bold text-text-muted">{authorInitials}</span>
                                )}
                            </div>
                        </div>
                    </Link>
                    <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-gradient-to-br from-primary to-secondary rounded-full border-2 border-bg-primary" />
                </div>

                <div className="flex-1 min-w-0">
                    {/* Header */}
                    <div className="flex items-center gap-3 mb-3">
                        <Link href={`/users/${post.authorId}`} className="group/author flex items-center gap-3 relative z-10">
                            <span className="font-bold text-text-primary text-lg group-hover/author:text-primary transition-colors">
                                {authorName}
                            </span>
                        </Link>
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

                    {/* Media Grid */}
                    {post.media && post.media.length > 0 && (
                        <div className={`grid gap-2 mb-6 ${post.media.length === 1 ? 'grid-cols-1' :
                            post.media.length === 2 ? 'grid-cols-2' :
                                post.media.length === 3 ? 'grid-cols-2' : 'grid-cols-2'
                            }`}>
                            {post.media.map((media, index) => (
                                <div
                                    key={index}
                                    className={`relative rounded-xl overflow-hidden bg-black/20 ${post.media!.length === 3 && index === 0 ? 'row-span-2' : 'aspect-video'
                                        }`}
                                >
                                    {media.type === 'video' ? (
                                        <video
                                            src={media.url}
                                            controls
                                            className="w-full h-full object-cover"
                                        />
                                    ) : (
                                        <img
                                            src={media.url}
                                            alt="Post content"
                                            className="w-full h-full object-cover hover:scale-105 transition-transform duration-500"
                                        />
                                    )}
                                </div>
                            ))}
                        </div>
                    )}

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

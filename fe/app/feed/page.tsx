'use client';

import React, { useState, useEffect } from 'react';
import { PostCard } from '@/components/feed/PostCard';
import { CreatePostModal } from '@/components/feed/CreatePostModal';
import { postService } from '@/services/post.service';
import type { Post } from '@/types/api';

export default function FeedPage() {
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [posts, setPosts] = useState<Post[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    const fetchFeed = async () => {
        try {
            const data = await postService.getFeed();
            setPosts(data.items);
        } catch (error) {
            console.error('Failed to load feed', error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchFeed();
    }, []);

    const handleLike = async (id: string) => {
        // Optimistic update
        setPosts(prev => prev.map(p => {
            if (p.id === id) {
                const newIsLiked = !p.isLiked;
                return {
                    ...p,
                    isLiked: newIsLiked,
                    likeCount: newIsLiked ? p.likeCount + 1 : Math.max(0, p.likeCount - 1)
                };
            }
            return p;
        }));

        try {
            await postService.toggleLike(id);
        } catch (error) {
            // Revert on failure
            console.error('Like failed', error);
            fetchFeed();
        }
    };

    return (
        <div className="w-full max-w-3xl mx-auto">

            {/* Create Post Trigger */}
            <div
                onClick={() => setIsCreateModalOpen(true)}
                className="glass-base rounded-2xl p-4 mb-8 flex items-center gap-4 cursor-pointer hover:bg-white/10 transition-colors group"
            >
                <div className="w-10 h-10 rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] p-[2px]">
                    <div className="w-full h-full rounded-full bg-[var(--color-bg-deep)] flex items-center justify-center text-xs font-bold">
                        Me
                    </div>
                </div>
                <div className="flex-1 h-10 rounded-full bg-white/5 flex items-center px-4 text-[var(--color-text-muted)] group-hover:text-white/80 transition-colors">
                    What's on your mind?
                </div>
                <div className="text-[var(--color-primary)] opacity-60 group-hover:opacity-100 transition-opacity p-2 rounded-full hover:bg-white/10">
                    <span className="text-xl">📷</span>
                </div>
            </div>

            {/* Feed List */}
            <div className="flex flex-col">
                {isLoading ? (
                    <div className="flex justify-center py-12">
                        <div className="flex items-center gap-2 text-[var(--color-text-muted)] animate-pulse">
                            <span className="w-2 h-2 rounded-full bg-[var(--color-primary)]" />
                            <span className="w-2 h-2 rounded-full bg-[var(--color-secondary)] delay-75" />
                            <span className="w-2 h-2 rounded-full bg-[var(--color-primary)] delay-150" />
                        </div>
                    </div>
                ) : (
                    posts.map(post => (
                        <PostCard
                            key={post.id}
                            post={post}
                            onLike={handleLike}
                        />
                    ))
                )}
            </div>

            <CreatePostModal
                isOpen={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                onSuccess={fetchFeed}
            />
        </div>
    );
}

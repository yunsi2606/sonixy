'use client';

import React, { useState, useEffect } from 'react';
import { PostCard } from '@/components/common/PostCard';
import { CreatePostModal } from '@/components/feed/CreatePostModal';
import { PostSkeleton } from '@/components/skeletons/PostSkeleton';
import { postService } from '@/services/post.service';
import type { Post } from '@/types/api';
import { Loader2 } from 'lucide-react';
import { socialService } from '@/services/social.service';

export default function FeedPage() {
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [posts, setPosts] = useState<Post[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isLoadingMore, setIsLoadingMore] = useState(false);
    const [nextCursor, setNextCursor] = useState<string | null>(null);
    const [hasMore, setHasMore] = useState(true);

    const fetchFeed = async (cursor?: string) => {
        if (!hasMore && cursor) return;

        try {
            if (cursor) {
                setIsLoadingMore(true);
            } else {
                setIsLoading(true);
            }

            const data = await postService.getFeed(cursor);

            if (cursor) {
                setPosts(prev => [...prev, ...data.items]);
            } else {
                setPosts(data.items);
            }

            setNextCursor(data.nextCursor);
            setHasMore(data.hasMore);
        } catch (error) {
            console.error('Failed to load feed', error);
        } finally {
            setIsLoading(false);
            setIsLoadingMore(false);
        }
    };

    useEffect(() => {
        fetchFeed();
    }, []);

    // Intersection Observer for Infinite Scroll
    const observerTarget = React.useRef<HTMLDivElement>(null);

    useEffect(() => {
        const observer = new IntersectionObserver(
            entries => {
                if (entries[0].isIntersecting && hasMore && !isLoading && !isLoadingMore) {
                    if (nextCursor) fetchFeed(nextCursor);
                }
            },
            { threshold: 0.1 }
        );

        if (observerTarget.current) {
            observer.observe(observerTarget.current);
        }

        return () => observer.disconnect();
    }, [hasMore, isLoading, isLoadingMore, nextCursor]);

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
            await socialService.toggleLike(id)
        } catch (error) {
            // Revert on failure
            console.error('Like failed', error);
            // Ideally we shouldn't refetch the whole feed on a like error to avoid resetting pagination,
            // just revert the local state back to what it was.
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
                    <div className="space-y-4">
                        <PostSkeleton />
                        <PostSkeleton />
                        <PostSkeleton />
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

                {/* Infinite Scroll Target */}
                <div ref={observerTarget} className="py-8 flex justify-center w-full">
                    {isLoadingMore && (
                        <div className="flex flex-col items-center gap-2">
                            <Loader2 className="w-8 h-8 text-[var(--color-primary)] animate-spin" />
                            <span className="text-sm font-medium text-[var(--color-text-muted)] animate-pulse">Loading more posts...</span>
                        </div>
                    )}

                    {!hasMore && posts.length > 0 && !isLoading && (
                        <div className="flex flex-col items-center gap-2 opacity-50">
                            <div className="w-2 h-2 rounded-full bg-[var(--color-text-muted)]" />
                            <span className="text-sm font-medium text-[var(--color-text-muted)]">You've reached the end</span>
                        </div>
                    )}
                </div>
            </div>

            <CreatePostModal
                isOpen={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                onSuccess={() => fetchFeed()} // Auto refresh to top on successful post
            />
        </div>
    );
}

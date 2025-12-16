'use client';

import React, { useState } from 'react';
import { PostCard } from '@/components/feed/PostCard';
import { CreatePostModal } from '@/components/feed/CreatePostModal';
import type { Post } from '@/types/api';

// Mock Data with specific intents
const MOCK_POSTS: (Post & { variant?: 'default' | 'hero' | 'compact' })[] = [
    {
        id: 'hero-1',
        authorId: 'admin',
        content: "Welcome to the new Sonixy Design System.\n\nExperience depth, subtle motion, and true glassmorphism.",
        likeCount: 1204,
        createdAt: new Date().toISOString(),
        visibility: 'public',
        updatedAt: new Date().toISOString(),
        variant: 'hero'
    },
    {
        id: '1',
        authorId: 'u1',
        content: "Just trying out the new layout! The sidebar navigation is so much smoother on desktop. 🚀",
        likeCount: 42,
        createdAt: new Date(Date.now() - 3600000).toISOString(),
        visibility: 'public',
        updatedAt: new Date().toISOString(),
    },
    {
        id: '2',
        authorId: 'u2',
        content: "Design tip: Use 'backdrop-filter: blur(20px)' sparingly. Too much blur kills performance on mobile browsers.",
        likeCount: 89,
        createdAt: new Date(Date.now() - 7200000).toISOString(),
        visibility: 'public',
        updatedAt: new Date().toISOString(),
    },
    {
        id: '3',
        authorId: 'u3',
        content: "Anyone know how to center a div in 2025? Asking for a friend.",
        likeCount: 15,
        createdAt: new Date(Date.now() - 172800000).toISOString(),
        visibility: 'public',
        updatedAt: new Date().toISOString(),
        variant: 'compact'
    }
];

export default function FeedPage() {
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [posts, setPosts] = useState(MOCK_POSTS);

    const handleLike = (id: string) => {
        setPosts(prev => prev.map(p => p.id === id ? { ...p, likeCount: p.likeCount + 1 } : p));
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
                {posts.map(post => (
                    <PostCard
                        key={post.id}
                        post={post}
                        variant={post.variant}
                        onLike={handleLike}
                    />
                ))}

                {/* Loading */}
                <div className="flex justify-center py-12">
                    <div className="flex items-center gap-2 text-[var(--color-text-muted)] animate-pulse">
                        <span className="w-2 h-2 rounded-full bg-[var(--color-primary)]" />
                        <span className="w-2 h-2 rounded-full bg-[var(--color-secondary)] delay-75" />
                        <span className="w-2 h-2 rounded-full bg-[var(--color-primary)] delay-150" />
                    </div>
                </div>
            </div>

            <CreatePostModal
                isOpen={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
            />
        </div>
    );
}

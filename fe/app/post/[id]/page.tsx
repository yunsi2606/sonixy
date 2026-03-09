'use client';

import React, { useState, useEffect } from 'react';
import { PostCard } from '@/components/common/PostCard';
import { CommentSection } from '@/components/comments/CommentSection';
import { PostSkeleton } from '@/components/skeletons/PostSkeleton';
import { CommentThreadSkeleton } from '@/components/skeletons/CommentSkeleton';
import { postService } from '@/services/post.service';
import type { Post } from '@/types/api';
import { ArrowLeft } from 'lucide-react';
import { useRouter } from 'next/navigation';

export default function PostDetailPage({ params }: { params: { id: string } }) {
    const [post, setPost] = useState<Post | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const router = useRouter();

    const fetchPost = async () => {
        try {
            const fetchedPost = await postService.getPostById(params.id);
            setPost(fetchedPost);
        } catch (error) {
            console.error('Failed to load post', error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchPost();
    }, [params.id]);

    const handleLike = async (id: string) => {
        if (!post) return;
        
        // Optimistic update
        const newIsLiked = !post.isLiked;
        setPost({
            ...post,
            isLiked: newIsLiked,
            likeCount: newIsLiked ? post.likeCount + 1 : Math.max(0, post.likeCount - 1)
        });

        try {
            await postService.toggleLike(id);
        } catch (error) {
            // Revert on failure
            console.error('Like failed', error);
            fetchPost();
        }
    };

    if (isLoading) {
        return (
            <div className="w-full max-w-2xl mx-auto space-y-4">
                <PostSkeleton />
                <CommentThreadSkeleton />
            </div>
        );
    }

    if (!post) {
        return (
            <div className="w-full max-w-2xl mx-auto text-center py-20">
                <h2 className="text-2xl font-bold text-white mb-2">Post not found</h2>
                <p className="text-[var(--color-text-muted)] mb-6">The post you are looking for doesn't exist or has been deleted.</p>
                <button 
                    onClick={() => router.back()}
                    className="px-6 py-2 bg-white/10 hover:bg-white/20 text-white rounded-xl transition-colors"
                >
                    Go Back
                </button>
            </div>
        );
    }

    return (
        <div className="w-full max-w-2xl mx-auto flex flex-col min-h-screen">
            <div className="mb-6 flex items-center gap-4">
                <button 
                    onClick={() => router.back()}
                    className="p-2 glass-base rounded-full hover:bg-white/10 transition-colors text-white"
                >
                    <ArrowLeft size={20} />
                </button>
                <h1 className="text-xl font-bold text-white">Post</h1>
            </div>

            <PostCard 
                post={post} 
                onLike={handleLike} 
                variant="hero" 
                disableCommentsInline={true}
            />

            <div className="flex-1 mt-4 glass-base rounded-2xl overflow-hidden border border-[var(--glass-border)] pb-24">
                <CommentSection post={post} />
            </div>
        </div>
    );
}

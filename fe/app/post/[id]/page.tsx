'use client';

import React, { useState, useEffect } from 'react';
import { PostSkeleton } from '@/components/skeletons/PostSkeleton';
import { CommentThreadSkeleton } from '@/components/skeletons/CommentSkeleton';
import { postService } from '@/services/post.service';
import type { Post } from '@/types/api';
import { ArrowLeft } from 'lucide-react';
import { useRouter, useParams } from 'next/navigation';
import { Lightbox } from '@/components/common/Lightbox';
import { TextPostModal } from '@/components/common/TextPostModal';

export default function PostDetailPage() {
    const { id } = useParams() as { id: string };
    const [post, setPost] = useState<Post | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const router = useRouter();

    const fetchPost = async () => {
        if (!id) return;
        try {
            const fetchedPost = await postService.getPostById(id);
            setPost(fetchedPost);
        } catch (error) {
            console.error('Failed to load post', error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (id) {
            fetchPost();
        }
    }, [id]);

    const handleLike = async (postId: string) => {
        if (!post) return;

        // Optimistic update
        const newIsLiked = !post.isLiked;
        setPost({
            ...post,
            isLiked: newIsLiked,
            likeCount: newIsLiked ? post.likeCount + 1 : Math.max(0, post.likeCount - 1)
        });

        try {
            await postService.toggleLike(postId);
        } catch (error) {
            console.error('Like failed', error);
            fetchPost();
        }
    };

    if (isLoading) {
        return (
            <div className="fixed inset-0 z-[100] bg-[var(--color-bg-base)] flex flex-col items-center justify-center p-4">
                <div className="w-full max-w-2xl space-y-4">
                    <PostSkeleton />
                    <CommentThreadSkeleton />
                </div>
            </div>
        );
    }

    if (!post) {
        return (
            <div className="fixed inset-0 z-[100] bg-[var(--color-bg-base)] flex flex-col items-center justify-center text-center p-4">
                <h2 className="text-2xl font-bold text-white mb-2">Post not found</h2>
                <p className="text-[var(--color-text-muted)] mb-6">The post you are looking for doesn't exist or has been deleted.</p>
                <button
                    onClick={() => router.back()}
                    className="px-6 py-2 bg-white/10 hover:bg-white/20 text-white rounded-xl transition-colors flex items-center gap-2 mx-auto"
                >
                    <ArrowLeft size={16} /> Go Back
                </button>
            </div>
        );
    }

    const isTextOnly = !post.media || post.media.length === 0;

    if (isTextOnly) {
        return (
            <TextPostModal 
                isOpen={true} 
                onClose={() => router.back()} 
                post={post}
                onLike={handleLike}
            />
        );
    }

    return (
        <Lightbox 
            isOpen={true} 
            onClose={() => router.back()} 
            post={post} 
        />
    );
}

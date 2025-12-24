import React, { useState, useEffect } from 'react';
import { CommentItem } from './CommentItem';
import { CommentInput } from './CommentInput';
import type { Post, User } from '@/types/api';
import type { Comment } from '@/types/comment';
import { commentService } from '@/services/comment.service';
import { userService } from '@/services/user.service';
import { CommentThreadSkeleton } from '@/components/skeletons/CommentSkeleton';

interface CommentSectionProps {
    post: Post;
    initialComments?: Comment[];
}

export function CommentSection({ post, initialComments = [] }: CommentSectionProps) {
    const [comments, setComments] = useState<Comment[]>(initialComments);
    const [currentUser, setCurrentUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    // Reply state
    const [replyingTo, setReplyingTo] = useState<{ commentId: string; username: string; rootId: string; userId: string } | null>(null);

    // Fetch data on mount
    useEffect(() => {
        const loadData = async () => {
            try {
                // Fetch User
                try {
                    const user = await userService.getCurrentUser();
                    setCurrentUser(user);
                } catch (e) {
                    console.error("Failed to fetch user (might not be logged in)", e);
                }

                // Fetch Comments if not provided or to refresh
                if (initialComments.length === 0) {
                    setIsLoading(true);
                    const commentsPage = await commentService.getComments(post.id);
                    setComments(commentsPage.items);
                }
            } catch (error) {
                console.error("Failed to load comments", error);
            } finally {
                setIsLoading(false);
            }
        };

        loadData();
    }, [post.id]);

    // --- Flattening / Grouping Logic ---
    const rootComments = comments.filter(c => c.parentId === null).sort((a, b) =>
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime() // Newest roots first
    );

    const getReplies = (rootId: string) => {
        return comments.filter(c => c.parentId === rootId).sort((a, b) =>
            new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime() // Oldest replies first within thread
        );
    };

    // --- Handlers ---
    const handleReply = (comment: Comment) => {
        const rootId = comment.parentId === null ? comment.id : comment.parentId;
        setReplyingTo({
            commentId: comment.id,
            username: comment.author.username,
            rootId: rootId,
            userId: comment.author.id
        });
    };

    const handleCancelReply = () => {
        setReplyingTo(null);
    };

    const handleSubmit = async (text: string) => {
        if (!currentUser) {
            alert("Please log in to comment.");
            return;
        }

        try {
            const newComment = await commentService.createComment({
                postId: post.id,
                content: text,
                authorUsername: currentUser.displayName || currentUser.email.split('@')[0],
                authorAvatarUrl: currentUser.avatarUrl || '',
                parentId: replyingTo ? replyingTo.rootId : undefined,
                replyToUserId: replyingTo ? replyingTo.userId : undefined,
                replyToUsername: replyingTo ? replyingTo.username : undefined
            });

            setComments(prev => [newComment, ...prev]);
            setReplyingTo(null);
        } catch (error) {
            console.error("Failed to post comment", error);
            alert("Failed to post comment. Please try again.");
        }
    };

    return (
        <div className="flex flex-col h-full bg-[var(--color-bg-surface)] border-l border-[var(--glass-border)]">
            {/* Header / Stats */}
            <div className="p-4 border-b border-[var(--glass-border)] flex items-center justify-between">
                <h3 className="font-semibold text-white">Comments</h3>
                <span className="text-xs text-white/50">{comments.length} comments</span>
            </div>

            {/* Scrollable List */}
            <div className="flex-1 overflow-y-auto p-4 space-y-6">
                {isLoading && comments.length === 0 ? (
                    <div className="mt-4 px-2">
                        <CommentThreadSkeleton />
                    </div>
                ) : rootComments.length === 0 ? (
                    <div className="text-center text-white/40 mt-10">
                        No comments yet. Be the first!
                    </div>
                ) : (
                    rootComments.map(root => (
                        <div key={root.id}>
                            {/* Root Comment */}
                            <CommentItem
                                comment={root}
                                onReply={handleReply}
                            />

                            {/* Replies Container */}
                            <div className="mt-1 space-y-1">
                                {getReplies(root.id).map(reply => (
                                    <CommentItem
                                        key={reply.id}
                                        comment={reply}
                                        onReply={handleReply}
                                        isReply={true}
                                    />
                                ))}
                            </div>
                        </div>
                    ))
                )}
            </div>

            {/* Input Area */}
            <CommentInput
                onSubmit={handleSubmit}
                replyingTo={replyingTo ? { id: replyingTo.commentId, username: replyingTo.username } : null}
                onCancelReply={handleCancelReply}
            />
        </div>
    );
}

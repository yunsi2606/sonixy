'use client';

import { useState } from 'react';
import { postService } from '@/services/post.service';

interface CreatePostModalProps {
    isOpen: boolean;
    onClose: () => void;
    onPostCreated?: () => void;
}

export function CreatePostModal({ isOpen, onClose, onPostCreated }: CreatePostModalProps) {
    const [content, setContent] = useState('');
    const [visibility, setVisibility] = useState<'public' | 'followers'>('public');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');

    const maxLength = 500;
    const remainingChars = maxLength - content.length;

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!content.trim()) {
            setError('Post content cannot be empty');
            return;
        }

        setIsLoading(true);
        setError('');

        try {
            await postService.createPost(content, visibility);
            setContent('');
            onPostCreated?.();
            onClose();
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to create post');
        } finally {
            setIsLoading(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div
            className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-md animate-[var(--animate-fade-in)]"
            onClick={onClose}
        >
            <div
                className="w-full max-w-2xl glass-strong rounded-2xl shadow-2xl animate-[var(--animate-scale-in)] gradient-border overflow-hidden"
                onClick={(e) => e.stopPropagation()}
            >
                {/* Header */}
                <div className="flex items-center justify-between p-6 border-b border-border">
                    <h2 className="text-3xl font-black gradient-text-vibrant">Create Post</h2>
                    <button
                        onClick={onClose}
                        className="w-10 h-10 flex items-center justify-center rounded-full hover:bg-surface transition-colors text-text-muted hover:text-text-primary"
                    >
                        <span className="text-2xl">✕</span>
                    </button>
                </div>

                {/* Form */}
                <form onSubmit={handleSubmit} className="p-6 space-y-6">
                    {/* User Info */}
                    <div className="flex items-center gap-3">
                        <div className="w-12 h-12 rounded-full bg-gradient-to-br from-primary via-secondary to-accent p-0.5">
                            <div className="w-full h-full rounded-full bg-bg-secondary flex items-center justify-center text-xl">
                                👤
                            </div>
                        </div>
                        <div>
                            <p className="font-bold text-text-primary">User</p>
                            <p className="text-sm text-text-muted">Share your thoughts...</p>
                        </div>
                    </div>

                    {/* Textarea */}
                    <div className="relative">
                        <textarea
                            value={content}
                            onChange={(e) => setContent(e.target.value.slice(0, maxLength))}
                            placeholder="What's on your mind? ✨"
                            rows={8}
                            className="w-full px-5 py-4 bg-surface/50 border-2 border-border rounded-xl focus:border-border-focus transition-all resize-none text-lg placeholder:text-text-muted"
                            maxLength={maxLength}
                        />
                        <div className={`absolute bottom-3 right-3 text-sm font-semibold ${remainingChars < 50 ? 'text-accent' : 'text-text-muted'
                            }`}>
                            {remainingChars}
                        </div>
                    </div>

                    {/* Visibility Selector */}
                    <div className="space-y-3">
                        <label className="text-sm font-bold text-text-primary uppercase tracking-wide">
                            Visibility
                        </label>
                        <div className="flex gap-3">
                            <button
                                type="button"
                                onClick={() => setVisibility('public')}
                                className={`flex-1 px-6 py-3 rounded-xl font-semibold transition-all duration-200 ${visibility === 'public'
                                    ? 'bg-gradient-to-r from-primary to-secondary text-white shadow-glow'
                                    : 'bg-surface text-text-secondary hover:bg-surface-hover'
                                    }`}
                            >
                                <span className="mr-2">🌍</span>
                                Public
                            </button>
                            <button
                                type="button"
                                onClick={() => setVisibility('followers')}
                                className={`flex-1 px-6 py-3 rounded-xl font-semibold transition-all duration-200 ${visibility === 'followers'
                                    ? 'bg-gradient-to-r from-primary to-secondary text-white shadow-glow'
                                    : 'bg-surface text-text-secondary hover:bg-surface-hover'
                                    }`}
                            >
                                <span className="mr-2">👥</span>
                                Followers
                            </button>
                        </div>
                    </div>

                    {/* Error Message */}
                    {error && (
                        <div className="card bg-red-500/10 border-red-500/30 p-4 animate-[var(--animate-scale-in)]">
                            <div className="flex items-center gap-2">
                                <span className="text-xl">⚠️</span>
                                <p className="text-red-400 font-medium">{error}</p>
                            </div>
                        </div>
                    )}

                    {/* Action Buttons */}
                    <div className="flex justify-end gap-3 pt-4 border-t border-border">
                        <button
                            type="button"
                            onClick={onClose}
                            className="btn-secondary"
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            disabled={isLoading || !content.trim()}
                            className="btn-primary shadow-glow hover:shadow-glow-strong disabled:opacity-50 disabled:hover:shadow-none disabled:transform-none"
                        >
                            {isLoading ? (
                                <span className="flex items-center gap-2">
                                    <span className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                                    Posting...
                                </span>
                            ) : (
                                <span>Post 🚀</span>
                            )}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

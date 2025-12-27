'use client';

import { useState, useRef } from 'react';
import { postService } from '@/services/post.service';
import { X, User, Globe, Users, AlertTriangle, Image as ImageIcon, MapPin, Rocket } from 'lucide-react';

interface CreatePostModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSuccess?: () => void;
}

export function CreatePostModal({ isOpen, onClose, onSuccess }: CreatePostModalProps) {
    const [content, setContent] = useState('');
    const [visibility, setVisibility] = useState<'public' | 'followers'>('public');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');

    // Media Upload State
    const [mediaFiles, setMediaFiles] = useState<File[]>([]);
    const [previews, setPreviews] = useState<{ url: string, type: 'image' | 'video' }[]>([]);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const maxLength = 500;
    const remainingChars = maxLength - content.length;

    const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            const newFiles = Array.from(e.target.files);
            setMediaFiles(prev => [...prev, ...newFiles]);

            const newPreviews = newFiles.map(file => ({
                url: URL.createObjectURL(file),
                type: file.type.startsWith('video') ? 'video' as const : 'image' as const
            }));
            setPreviews(prev => [...prev, ...newPreviews]);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!content.trim() && mediaFiles.length === 0) {
            setError('Post content or media cannot be empty');
            return;
        }

        setIsLoading(true);
        setError('');

        try {
            await postService.createPost(content, visibility, mediaFiles);
            setContent('');
            setMediaFiles([]);
            setPreviews([]);
            onSuccess?.();
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
                className="w-full max-w-2xl glass-strong rounded-2xl shadow-2xl animate-[var(--animate-scale-in)] gradient-border overflow-hidden max-h-[90vh] flex flex-col"
                onClick={(e) => e.stopPropagation()}
            >
                {/* Header */}
                <div className="flex items-center justify-between p-6 border-b border-border shrink-0">
                    <h2 className="text-3xl font-black gradient-text-vibrant">Create Post</h2>
                    <button
                        onClick={onClose}
                        disabled={isLoading}
                        className="w-10 h-10 flex items-center justify-center rounded-full hover:bg-surface transition-colors text-text-muted hover:text-text-primary"
                    >
                        <X size={24} />
                    </button>
                </div>

                {/* Scrollable Content */}
                <div className="overflow-y-auto custom-scrollbar flex-1">
                    <form onSubmit={handleSubmit} className="p-6 space-y-6">
                        {/* User Info */}
                        <div className="flex items-center gap-3">
                            <div className="w-12 h-12 rounded-full bg-gradient-to-br from-primary via-secondary to-accent p-0.5">
                                <div className="w-full h-full rounded-full bg-bg-secondary flex items-center justify-center text-xl">
                                    <User size={24} />
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
                                rows={5}
                                className="w-full px-5 py-4 bg-surface/50 border-2 border-border rounded-xl focus:border-border-focus transition-all resize-none text-lg placeholder:text-text-muted"
                                maxLength={maxLength}
                                disabled={isLoading}
                            />
                            <div className={`absolute bottom-3 right-3 text-sm font-semibold ${remainingChars < 50 ? 'text-accent' : 'text-text-muted'}`}>
                                {remainingChars}
                            </div>
                        </div>

                        {/* Media Previews */}
                        {previews.length > 0 && (
                            <div className="grid grid-cols-2 gap-3 mt-4 animate-fade-in">
                                {previews.map((preview, idx) => (
                                    <div key={idx} className="relative aspect-video rounded-xl overflow-hidden bg-black/20 border border-border group">
                                        {preview.type === 'video' ? (
                                            <video src={preview.url} className="w-full h-full object-cover" controls />
                                        ) : (
                                            <img src={preview.url} alt="Preview" className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
                                        )}
                                        <button
                                            type="button"
                                            onClick={() => {
                                                setMediaFiles(prev => prev.filter((_, i) => i !== idx));
                                                setPreviews(prev => prev.filter((_, i) => i !== idx));
                                            }}
                                            className="absolute top-2 right-2 w-8 h-8 rounded-full bg-black/50 text-white flex items-center justify-center hover:bg-red-500/80 transition-colors opacity-0 group-hover:opacity-100"
                                        >
                                            <X size={16} />
                                        </button>
                                    </div>
                                ))}
                            </div>
                        )}

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
                                    <Globe size={18} className="mr-2" />
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
                                    <Users size={18} className="mr-2" />
                                    Followers
                                </button>
                            </div>
                        </div>

                        {/* Error Message */}
                        {error && (
                            <div className="card bg-red-500/10 border-red-500/30 p-4 animate-[var(--animate-scale-in)]">
                                <div className="flex items-center gap-2">
                                    <AlertTriangle size={24} className="text-red-400" />
                                    <p className="text-red-400 font-medium">{error}</p>
                                </div>
                            </div>
                        )}

                        {/* Action Buttons */}
                        <div className="flex items-center justify-between pt-4 border-t border-border">
                            <div className="flex gap-3">
                                <input
                                    type="file"
                                    multiple
                                    accept="image/*,video/*"
                                    className="hidden"
                                    ref={fileInputRef}
                                    onChange={handleFileSelect}
                                />
                                <button
                                    type="button"
                                    onClick={() => fileInputRef.current?.click()}
                                    className="w-12 h-12 rounded-xl flex items-center justify-center bg-surface hover:bg-primary/20 hover:text-primary transition-all group"
                                    title="Add Media"
                                >
                                    <ImageIcon size={24} className="group-hover:scale-110 transition-transform" />
                                </button>
                                <button
                                    type="button"
                                    className="w-12 h-12 rounded-xl flex items-center justify-center bg-surface hover:bg-secondary/20 hover:text-secondary transition-all group"
                                    title="Add Location"
                                >
                                    <MapPin size={24} className="group-hover:scale-110 transition-transform" />
                                </button>
                            </div>

                            <div className="flex gap-3">
                                <button
                                    type="button"
                                    onClick={onClose}
                                    className="btn-secondary"
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    disabled={isLoading || (!content.trim() && mediaFiles.length === 0)}
                                    className="btn-primary shadow-glow hover:shadow-glow-strong disabled:opacity-50 disabled:hover:shadow-none disabled:transform-none px-8"
                                >
                                    {isLoading ? (
                                        <span className="flex items-center gap-2">
                                            <span className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                                            Posting...
                                        </span>
                                    ) : (
                                        <span className="flex items-center gap-2">Post <Rocket size={18} /></span>
                                    )}
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}

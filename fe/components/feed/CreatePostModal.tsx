import React, { useState, useRef } from 'react';
import { postService } from '@/services/post.service';

interface CreatePostModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSuccess?: () => void;
}

export function CreatePostModal({ isOpen, onClose, onSuccess }: CreatePostModalProps) {
    const [content, setContent] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [mediaFiles, setMediaFiles] = useState<File[]>([]);
    const [previews, setPreviews] = useState<{ url: string, type: 'image' | 'video' }[]>([]);
    const fileInputRef = useRef<HTMLInputElement>(null);

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

    const handleSubmit = async () => {
        if (!content.trim() && mediaFiles.length === 0) return;

        setIsLoading(true);
        try {
            await postService.createPost(content, 'public', mediaFiles);
            setContent('');
            setMediaFiles([]);
            setPreviews([]);
            onSuccess?.();
            onClose();
        } catch (error) {
            console.error('Failed to create post:', error);
            alert('Failed to post. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
            {/* Backdrop */}
            <div
                className="absolute inset-0 bg-black/60 backdrop-blur-[8px] animate-[fade-in_0.2s_ease-out]"
                onClick={onClose}
            />

            {/* Modal */}
            <div className="relative w-full max-w-lg glass-overlay rounded-2xl shadow-lift overflow-hidden animate-[scale-in_0.3s_cubic-bezier(0.16,1,0.3,1)]">
                {/* Header */}
                <div className="flex items-center justify-between p-4 border-b border-[var(--glass-border)]">
                    <h2 className="text-lg font-bold text-white">Create Post</h2>
                    <button
                        onClick={onClose}
                        disabled={isLoading}
                        className="p-2 hover:bg-white/10 rounded-full transition-colors text-white/70 hover:text-white"
                    >
                        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                {/* Content */}
                <div className="p-4 max-h-[60vh] overflow-y-auto custom-scrollbar">
                    <div className="flex gap-3">
                        <div className="w-10 h-10 rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] p-[2px] flex-shrink-0">
                            <div className="w-full h-full rounded-full bg-[var(--color-bg-deep)]" />
                        </div>
                        <div className="flex-1">
                            <textarea
                                placeholder="What's on your mind?"
                                className="w-full bg-transparent text-lg text-white placeholder-white/30 resize-none outline-none min-h-[100px]"
                                value={content}
                                onChange={(e) => setContent(e.target.value)}
                                disabled={isLoading}
                            />

                            {/* Media Previews */}
                            {previews.length > 0 && (
                                <div className="grid grid-cols-2 gap-2 mt-4">
                                    {previews.map((preview, idx) => (
                                        <div key={idx} className="relative aspect-video rounded-lg overflow-hidden bg-white/5 border border-[var(--glass-border)]">
                                            {preview.type === 'video' ? (
                                                <video src={preview.url} className="w-full h-full object-cover" />
                                            ) : (
                                                <img src={preview.url} alt="Preview" className="w-full h-full object-cover" />
                                            )}
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    </div>
                </div>

                {/* Footer Actions */}
                <div className="flex items-center justify-between p-4 border-t border-[var(--glass-border)] bg-white/5">
                    <div className="flex items-center gap-2">
                        <input
                            type="file"
                            multiple
                            accept="image/*,video/*"
                            className="hidden"
                            ref={fileInputRef}
                            onChange={handleFileSelect}
                        />
                        <button
                            onClick={() => fileInputRef.current?.click()}
                            disabled={isLoading}
                            className="p-2 text-[var(--color-primary)] hover:bg-[var(--color-primary)]/10 rounded-full transition-colors"
                        >
                            <span className="sr-only">Media</span>
                            📷/🎥
                        </button>
                    </div>

                    <button
                        onClick={handleSubmit}
                        disabled={isLoading || (!content.trim() && mediaFiles.length === 0)}
                        className="btn-primary flex items-center gap-2 px-6 py-2 shadow-[var(--shadow-neon)] hover:shadow-[0_0_30px_rgba(0,229,255,0.4)] transition-shadow disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {isLoading ? 'Posting...' : 'Post'}
                    </button>
                </div>
            </div>
        </div>
    );
}

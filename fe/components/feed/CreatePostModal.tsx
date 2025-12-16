import React, { useState } from 'react';

interface CreatePostModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export function CreatePostModal({ isOpen, onClose }: CreatePostModalProps) {
    const [content, setContent] = useState('');

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
                        className="p-2 hover:bg-white/10 rounded-full transition-colors text-white/70 hover:text-white"
                    >
                        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                {/* Content */}
                <div className="p-4">
                    <div className="flex gap-3">
                        <div className="w-10 h-10 rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] p-[2px] flex-shrink-0">
                            <div className="w-full h-full rounded-full bg-[var(--color-bg-deep)]" />
                        </div>
                        <textarea
                            placeholder="What's on your mind?"
                            className="w-full bg-transparent text-lg text-white placeholder-white/30 resize-none outline-none min-h-[120px]"
                            value={content}
                            onChange={(e) => setContent(e.target.value)}
                        />
                    </div>
                </div>

                {/* Footer Actions */}
                <div className="flex items-center justify-between p-4 border-t border-[var(--glass-border)] bg-white/5">
                    <div className="flex items-center gap-2">
                        <button className="p-2 text-[var(--color-primary)] hover:bg-[var(--color-primary)]/10 rounded-full transition-colors">
                            <span className="sr-only">Image</span>
                            📷
                        </button>
                        <button className="p-2 text-[var(--color-secondary)] hover:bg-[var(--color-secondary)]/10 rounded-full transition-colors">
                            <span className="sr-only">Video</span>
                            🎥
                        </button>
                    </div>

                    <button className="btn-primary flex items-center gap-2 px-6 py-2 shadow-[var(--shadow-neon)] hover:shadow-[0_0_30px_rgba(0,229,255,0.4)] transition-shadow">
                        Post
                    </button>
                </div>
            </div>
        </div>
    );
}

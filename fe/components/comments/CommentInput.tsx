import React, { useState, useEffect, useRef } from 'react';

interface CommentInputProps {
    onSubmit: (text: string) => void;
    replyingTo?: { username: string; id: string } | null;
    onCancelReply: () => void;
}

export function CommentInput({ onSubmit, replyingTo, onCancelReply }: CommentInputProps) {
    const [text, setText] = useState('');
    const inputRef = useRef<HTMLInputElement>(null);

    // Auto-focus when entering reply mode
    useEffect(() => {
        if (replyingTo && inputRef.current) {
            inputRef.current.focus();
        }
    }, [replyingTo]);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!text.trim()) return;
        onSubmit(text);
        setText('');
    };

    return (
        <div className="p-3 border-t border-[var(--glass-border)] bg-[var(--color-bg-deep)]/95 backdrop-blur-md">
            {replyingTo && (
                <div className="flex items-center justify-between text-xs text-white/50 mb-2 px-1">
                    <span>
                        Replying to <span className="text-[var(--color-primary)] font-medium">@{replyingTo.username}</span>
                    </span>
                    <button
                        onClick={onCancelReply}
                        className="hover:text-white transition-colors"
                        title="Cancel reply"
                    >
                        ✕
                    </button>
                </div>
            )}

            <form onSubmit={handleSubmit} className="flex items-center gap-2">
                <div className="w-8 h-8 rounded-full bg-white/10 flex items-center justify-center text-xs text-white/50 flex-shrink-0">
                    Me
                </div>
                <div className="flex-1 relative">
                    <input
                        ref={inputRef}
                        type="text"
                        value={text}
                        onChange={(e) => setText(e.target.value)}
                        placeholder={replyingTo ? `Reply to ${replyingTo.username}...` : "Write a comment..."}
                        className="w-full bg-white/5 border border-white/10 rounded-full py-2 px-4 text-sm text-[var(--color-text-primary)] placeholder:text-white/20 focus:outline-none focus:border-[var(--color-primary)] transition-colors"
                    />
                </div>
                <button
                    type="submit"
                    disabled={!text.trim()}
                    className="p-2 text-[var(--color-primary)] hover:bg-[var(--color-primary)]/10 rounded-full transition-colors disabled:opacity-50 disabled:hover:bg-transparent"
                >
                    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><line x1="22" y1="2" x2="11" y2="13"></line><polygon points="22 2 15 22 11 13 2 9 22 2"></polygon></svg>
                </button>
            </form>
        </div>
    );
}

import React, { useState, useRef } from 'react';
import { Send, Image as ImageIcon, X } from 'lucide-react';
import { useChat } from '@/contexts/ChatContext';

interface MessageInputProps {
    conversationId: string;
    onSend: (content: string, type: 'text' | 'image') => Promise<void>;
}

export const MessageInput: React.FC<MessageInputProps> = ({ conversationId, onSend }) => {
    const [content, setContent] = useState('');
    const [isTyping, setIsTyping] = useState(false);
    const { connection } = useChat();
    const typingTimeoutRef = useRef<NodeJS.Timeout | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleSend = async () => {
        if (!content.trim()) return;
        
        await onSend(content, 'text');
        setContent('');
        stopTyping();
    };

    const handleKeyDown = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSend();
        }
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setContent(e.target.value);
        
        if (!isTyping) {
            setIsTyping(true);
            connection?.invoke('TypingStarted', conversationId).catch(console.error);
        }

        if (typingTimeoutRef.current) clearTimeout(typingTimeoutRef.current);
        
        typingTimeoutRef.current = setTimeout(() => {
            stopTyping();
        }, 2000);
    };

    const stopTyping = () => {
        setIsTyping(false);
        connection?.invoke('TypingStopped', conversationId).catch(console.error);
    };

    const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        // TODO: Implement image upload logic here similar to CreatePostModal
        // For now just console log
        if (e.target.files && e.target.files.length > 0) {
            console.log("Files selected:", e.target.files);
            // Ideally: Upload to MinIO -> Get URL -> Send Message with Type=Image
        }
    };

    return (
        <div className="p-4 border-t border-[var(--glass-border)] bg-black/5 backdrop-blur-md">
            <div className="flex items-center gap-3">
                <input
                    type="file"
                    className="hidden"
                    ref={fileInputRef}
                    accept="image/*"
                    onChange={handleFileSelect}
                />
                <button 
                    onClick={() => fileInputRef.current?.click()}
                    className="p-2 rounded-xl text-[var(--color-text-secondary)] hover:bg-[var(--color-surface)] hover:text-[var(--color-primary)] transition-colors"
                >
                    <ImageIcon size={20} />
                </button>
                
                <div className="flex-1 relative">
                    <input 
                        value={content}
                        onChange={handleChange}
                        onKeyDown={handleKeyDown}
                        placeholder="Type a message..." 
                        className="w-full px-4 py-2.5 bg-black/10 border border-[var(--glass-border)] rounded-full focus:border-[var(--color-primary)] focus:ring-1 focus:ring-[var(--color-primary)] transition-all outline-none text-sm placeholder:text-[var(--color-text-muted)] text-[var(--color-text-primary)]"
                    />
                </div>
                
                <button 
                    onClick={handleSend} 
                    disabled={!content.trim()} 
                    className="p-2.5 rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] text-white shadow-[var(--shadow-neon)] hover:opacity-90 disabled:opacity-50 disabled:shadow-none transition-all transform hover:scale-105"
                >
                    <Send size={18} className={content.trim() ? "ml-0.5" : ""} />
                </button>
            </div>
        </div>
    );
};

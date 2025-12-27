import React, { useEffect, useRef, useState } from 'react';
import { useChat } from '@/contexts/ChatContext';
import { MessageItem } from './MessageItem';
import { MessageInput } from './MessageInput';
import { chatService } from '@/services/chat.service';
import Cookies from 'js-cookie';
import { MessageType } from '@/types/chat';

interface ChatWindowProps {
    conversationId: string;
}

export const ChatWindow: React.FC<ChatWindowProps> = ({ conversationId }) => {
    const { messages, addMessage, typingUsers, userMap } = useChat();
    const currentUserId = Cookies.get('userId');

    const messagesEndRef = useRef<HTMLDivElement>(null);
    const [isLoading, setIsLoading] = useState(false);

    const conversationMessages = messages.get(conversationId) || [];
    const activeTypingUsers = typingUsers.get(conversationId) || [];

    useEffect(() => {
        // Only fetch if empty to avoid overwriting live updates, or improve logic to merge
        if (conversationMessages.length === 0) {
            setIsLoading(true);
            chatService.getMessages(conversationId)
                .then(msgs => {
                    // Prepend or set? The context logic was "append NEW", so we just Loop Add.
                    // This is inefficient but consistent with current Context logic.
                    // We reverse to add oldest first if getMessages returns newest first.
                    // Usually API returns [Newest...Oldest] or [Oldest...Newest]. 
                    // Let's assume API returns [Newest...Oldest] (standard desc).
                    // So we reverse to [Oldest...Newest] and add.
                    msgs.reverse().forEach(m => addMessage(conversationId, m));
                })
                .finally(() => setIsLoading(false));
        }
    }, [conversationId]);

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [conversationMessages, activeTypingUsers]);

    const handleSend = async (content: string, typeStr: 'text' | 'image') => {
        const type = typeStr === 'image' ? MessageType.Image : MessageType.Text;
        try {
            const sentMsg = await chatService.sendMessage({
                conversationId,
                content,
                type
            });
            addMessage(conversationId, sentMsg);
        } catch (err) {
            console.error('Failed to send', err);
        }
    };

    return (
        <div className="flex flex-col h-full bg-transparent">
            {/* Messages Area */}
            <div className="flex-1 overflow-y-auto p-4 custom-scrollbar">
                {isLoading && (
                    <div className="text-center text-[var(--color-text-muted)] text-xs py-4">
                        Loading messages...
                    </div>
                )}

                {conversationMessages.map((msg) => {
                    const sender = userMap.get(msg.senderId);
                    return (
                        <MessageItem
                            key={msg.id}
                            message={msg}
                            isOwn={msg.senderId === currentUserId}
                            senderName={sender?.displayName || sender?.username || "Chat User"}
                            senderAvatar={sender?.avatarUrl}
                        />
                    );
                })}


                {activeTypingUsers.length > 0 && (
                    <div className="flex items-center gap-2 mb-4 ml-2 animate-pulse">
                        <div className="w-8 h-8 rounded-full bg-[var(--color-surface)] flex items-center justify-center text-xs">...</div>
                        <div className="text-xs text-[var(--color-text-muted)] italic">Typing...</div>
                    </div>
                )}

                <div ref={messagesEndRef} />
            </div>

            {/* Input Area */}
            <div className="shrink-0">
                <MessageInput conversationId={conversationId} onSend={handleSend} />
            </div>
        </div>
    );
};

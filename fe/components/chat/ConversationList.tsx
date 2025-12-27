import React, { useEffect, useState } from 'react';
import { useChat } from '@/contexts/ChatContext';
import { chatService } from '@/services/chat.service';
import { Conversation, ConversationType } from '@/types/chat';
import { formatDistanceToNow } from 'date-fns';

interface ConversationListProps {
    onSelect?: () => void;
    compact?: boolean;
}

export const ConversationList: React.FC<ConversationListProps> = ({ onSelect, compact }) => {
    const { activeConversationId, setActiveConversationId, typingUsers } = useChat();
    const [conversations, setConversations] = useState<Conversation[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        chatService.getConversations()
            .then(setConversations)
            .finally(() => setIsLoading(false));
    }, []);

    const getDisplayInfo = (conv: Conversation) => {
        const name = conv.type === ConversationType.Group ? "Group Chat" : "User Name"; 
        return { name, avatar: undefined };
    };

    return (
        <div className={`flex flex-col h-full bg-transparent ${compact ? "w-full" : "w-80 border-r border-[var(--glass-border)]"}`}>
            {!compact && (
                <div className="p-4 border-b border-[var(--glass-border)] font-bold text-lg text-[var(--color-text-primary)]">
                    Messages
                </div>
            )}
            
            <div className="flex-1 overflow-y-auto custom-scrollbar">
                {isLoading && (
                    <div className="p-4 text-center text-[var(--color-text-muted)] text-sm">
                        Loading conversations...
                    </div>
                )}
                
                {conversations.map(conv => {
                    const { name, avatar } = getDisplayInfo(conv);
                    const isActive = activeConversationId === conv.id;
                    const isTyping = typingUsers.has(conv.id);

                    return (
                        <div 
                            key={conv.id}
                            onClick={() => {
                                setActiveConversationId(conv.id);
                                onSelect?.();
                            }}
                            className={`
                                flex items-center gap-3 p-3 cursor-pointer transition-all border-l-2
                                ${isActive 
                                    ? "bg-[var(--color-primary)]/10 border-[var(--color-primary)]" 
                                    : "border-transparent hover:bg-white/5"
                                }
                            `}
                        >
                            {/* Avatar */}
                            <div className="relative shrink-0">
                                <div className="w-10 h-10 rounded-full bg-gradient-to-br from-[var(--color-primary)] to-[var(--color-secondary)] p-0.5">
                                    <div className="w-full h-full rounded-full bg-[var(--color-bg-deep)] flex items-center justify-center overflow-hidden">
                                        {avatar ? (
                                            <img src={avatar} alt={name} className="w-full h-full object-cover" />
                                        ) : (
                                            <span className="font-bold text-sm text-[var(--color-text-primary)]">
                                                {name[0]}
                                            </span>
                                        )}
                                    </div>
                                </div>
                                {/* Online Status (Mock) */}
                                <div className="absolute bottom-0 right-0 w-3 h-3 rounded-full bg-green-500 border-2 border-[var(--color-bg-deep)]"></div>
                            </div>
                            
                            <div className="flex-1 min-w-0">
                                <div className="flex justify-between items-baseline mb-0.5">
                                    <span className={`font-semibold truncate text-sm ${isActive ? "text-[var(--color-primary)]" : "text-[var(--color-text-primary)]"}`}>
                                        {name}
                                    </span>
                                    <span className="text-[10px] text-[var(--color-text-muted)] flex-shrink-0 ml-2">
                                        {conv.lastMessageAt && formatDistanceToNow(new Date(conv.lastMessageAt), { addSuffix: false })}
                                    </span>
                                </div>
                                
                                <div className="text-xs text-[var(--color-text-secondary)] truncate h-4">
                                    {isTyping ? (
                                        <span className="text-[var(--color-primary)] italic">Typing...</span>
                                    ) : (
                                        <span>
                                            {conv.lastMessageSenderId === "me" ? "You: " : ""}
                                            {conv.lastMessageType === 1 ? "Sent an image" : conv.lastMessageContent || "Start chatting"}
                                        </span>
                                    )}
                                </div>
                            </div>
                            
                            {conv.unreadCount > 0 && (
                                <div className="min-w-[18px] h-[18px] px-1 rounded-full bg-[var(--color-primary)] text-white text-[10px] flex items-center justify-center font-bold">
                                    {conv.unreadCount}
                                </div>
                            )}
                        </div>
                    );
                })}
            </div>
        </div>
    );
};

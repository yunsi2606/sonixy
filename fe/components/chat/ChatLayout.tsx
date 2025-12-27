"use client";

import React from 'react';
import { ConversationList } from './ConversationList';
import { ChatWindow } from './ChatWindow';
import { useChat } from '@/contexts/ChatContext';

export const ChatLayout = () => {
    const { activeConversationId } = useChat();

    return (
        <div className="flex h-full w-full glass-strong rounded-2xl overflow-hidden border border-[var(--glass-border)] shadow-2xl">
            <ConversationList />
            
            <div className="flex-1 bg-[var(--color-bg-deep)]/30 backdrop-blur-sm flex flex-col min-w-0">
                {activeConversationId ? (
                    <ChatWindow conversationId={activeConversationId} />
                ) : (
                    <div className="flex-1 flex flex-col items-center justify-center text-[var(--color-text-muted)] gap-4">
                        <div className="w-20 h-20 rounded-full bg-white/5 flex items-center justify-center">
                            <span className="text-4xl">💬</span>
                        </div>
                        <p>Select a conversation to start chatting</p>
                    </div>
                )}
            </div>
        </div>
    );
};

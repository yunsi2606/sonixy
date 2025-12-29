"use client";

import React from 'react';
import { ConversationList } from './ConversationList';
import { ChatWindow } from './ChatWindow';
import { useChat } from '@/contexts/ChatContext';

export const ChatLayout = () => {
    const { activeConversationId } = useChat();

    return (
        <div className="flex h-full w-full bg-[#0B0F1A] rounded-2xl overflow-hidden border border-[var(--glass-border)] shadow-2xl relative">
            {/* Conversation List - Hidden on mobile if chat is active */}
            <ConversationList
                className={`${activeConversationId ? 'hidden md:flex' : 'flex w-full'} md:w-80 border-r border-[var(--glass-border)]`}
            />

            {/* Chat Window - Hidden on mobile if NO chat is active */}
            <div className={`${!activeConversationId ? 'hidden md:flex' : 'flex'} flex-1 bg-[var(--color-bg-deep)] flex-col min-w-0`}>
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

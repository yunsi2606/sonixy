"use client";

import React, { useState } from 'react';
import { MessageCircle, X, Maximize2 } from 'lucide-react';
import { useChat } from '@/contexts/ChatContext';
import { ChatWindow } from './ChatWindow';
import { ConversationList } from './ConversationList';
import Link from 'next/link';

export const GlobalChatWidget = () => {
    const [isOpen, setIsOpen] = useState(false);
    const { activeConversationId, setActiveConversationId, isConnected } = useChat();

    return (
        <div className={`fixed z-[100] flex flex-col items-end gap-4 ${isOpen ? 'inset-0 sm:inset-auto sm:bottom-6 sm:right-6' : 'bottom-20 right-4 sm:bottom-6 sm:right-6'}`}>
            {isOpen && (
                <div className="w-full h-full sm:w-[360px] sm:h-[520px] bg-[#0f111a] sm:rounded-2xl shadow-2xl border border-[var(--glass-border)] flex flex-col overflow-hidden animate-[var(--animate-scale-in)] origin-bottom-right">
                    {/* Header */}
                    <div className="h-14 bg-[#1a1d2d] border-b border-[var(--glass-border)] flex items-center justify-between px-4 shrink-0">
                        <span className="font-bold gradient-text-vibrant">Messages</span>
                        <div className="flex items-center gap-1">
                            <Link href="/messages" passHref>
                                <button className="p-2 rounded-lg text-white/70 hover:bg-white/10 hover:text-white transition-colors">
                                    <Maximize2 size={16} />
                                </button>
                            </Link>
                            <button
                                onClick={() => setIsOpen(false)}
                                className="p-2 rounded-lg text-white/70 hover:bg-white/10 hover:text-white transition-colors"
                            >
                                <X size={16} />
                            </button>
                        </div>
                    </div>

                    {/* Content */}
                    <div className="flex-1 flex overflow-hidden relative bg-[var(--color-bg-deep)]">
                        {activeConversationId ? (
                            <div className="absolute inset-0 flex flex-col animate-[var(--animate-fade-in)]">
                                <div className="bg-[#1a1d2d] border-b border-[var(--glass-border)] p-2 flex items-center">
                                    <button
                                        onClick={() => setActiveConversationId(null)}
                                        className="text-xs px-3 py-1.5 rounded-lg hover:bg-white/5 text-[var(--color-text-secondary)] font-medium transition-colors"
                                    >
                                        ← Back
                                    </button>
                                </div>
                                <div className="flex-1 overflow-hidden">
                                    <ChatWindow conversationId={activeConversationId} />
                                </div>
                            </div>
                        ) : (
                            <div className="w-full h-full animate-[var(--animate-fade-in)] bg-[#0f111a]">
                                <ConversationList compact onSelect={() => { }} />
                            </div>
                        )}
                    </div>
                </div>
            )}

            {/* Toggle Button */}
            <button
                onClick={() => setIsOpen(!isOpen)}
                className={`
                    w-14 h-14 rounded-full flex items-center justify-center shadow-[var(--shadow-neon)] transition-all duration-300
                    ${isOpen
                        ? "hidden sm:flex bg-[var(--color-surface)] text-[var(--color-text-primary)] rotate-90"
                        : "bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] text-white hover:scale-110"
                    }
                `}
            >
                {isOpen ? <X size={24} /> : <MessageCircle size={24} />}
            </button>
        </div>
    );
};

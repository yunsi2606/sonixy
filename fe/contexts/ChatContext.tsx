"use client";

import React, { createContext, useContext, useEffect, useState, ReactNode, useRef } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import Cookies from 'js-cookie';
import { Conversation, Message } from '@/types/chat';
import { User } from '@/types/api';

interface ChatContextType {
    connection: HubConnection | null;
    isConnected: boolean;
    activeConversationId: string | null;
    setActiveConversationId: (id: string | null) => void;
    typingUsers: Map<string, string[]>; // ConversationId -> UserIds
    messages: Map<string, Message[]>; // ConversationId -> Messages
    addMessage: (conversationId: string, message: Message) => void;
    userMap: Map<string, User>;
    upsertUsers: (users: Map<string, User>) => void;
    // Handlers
}

const ChatContext = createContext<ChatContextType | undefined>(undefined);

export const useChat = () => {
    const context = useContext(ChatContext);
    if (!context) {
        throw new Error('useChat must be used within a ChatProvider');
    }
    return context;
};

interface ChatProviderProps {
    children: ReactNode;
}

export const ChatProvider: React.FC<ChatProviderProps> = ({ children }) => {
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);
    const [activeConversationId, setActiveConversationId] = useState<string | null>(null);
    const [typingUsers, setTypingUsers] = useState<Map<string, string[]>>(new Map());

    // We store messages in a Map to cache history. 
    // Ideally use useQuery (TanStack Query) for data fetching, but manual for now.
    const [messages, setMessages] = useState<Map<string, Message[]>>(new Map());

    const isInitialized = useRef(false);

    useEffect(() => {
        if (isInitialized.current) return;
        isInitialized.current = true;

        const token = Cookies.get('accessToken');
        if (!token) return;

        const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5050';
        // Note: API_URL should match Gateway. Local dev gateway is usually 5050 or 8080?
        // Wait, Ocelot config said 8100? No, Ocelot config is JUST config.
        // The Gateway *Port* is determined by Gateway Project launch settings.
        // My Ocelot config used `5015` for chat service.

        // I should assume NEXT_PUBLIC_API_URL points to Gateway.

        const newConnection = new HubConnectionBuilder()
            .withUrl(`${apiUrl}/hubs/chat`, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log('SignalR Connected');
                    setIsConnected(true);
                })
                .catch(err => console.error('SignalR Connection Error: ', err));

            // Event Listeners
            connection.on('ReceiveMessage', (message: Message) => {
                const convId = message.conversationId;
                const currentUserId = Cookies.get('userId');

                // If I sent it, I already added it via optimistic update.
                // Just in case, we check ID too, but ignoring by Sender is safer for "echo" messages 
                // IF we trust optimistic update is always called.
                if (message.senderId === currentUserId) return;

                setMessages(prev => {
                    const existing = prev.get(convId) || [];
                    // Avoid duplicates by ID (double safety)
                    if (existing.some(m => m.id === message.id)) return prev;

                    const updated = [...existing, message]; // Append NEW message to end
                    // Sort if needed? Usually ordered by time.
                    return new Map(prev).set(convId, updated);
                });
            });

            connection.on('UserTyping', (conversationId: string, userId: string) => {
                setTypingUsers(prev => {
                    const exist = prev.get(conversationId) || [];
                    if (!exist.includes(userId)) {
                        return new Map(prev).set(conversationId, [...exist, userId]);
                    }
                    return prev;
                });
            });

            connection.on('UserStoppedTyping', (conversationId: string, userId: string) => {
                setTypingUsers(prev => {
                    const exist = prev.get(conversationId) || [];
                    return new Map(prev).set(conversationId, exist.filter(id => id !== userId));
                });
            });

            return () => {
                connection.stop();
            };
        }
    }, [connection]);

    const addMessage = (conversationId: string, message: Message) => {
        setMessages(prev => {
            const existing = prev.get(conversationId) || [];
            if (existing.some(m => m.id === message.id)) return prev;
            return new Map(prev).set(conversationId, [...existing, message]);
        });
    };

    const [userMap, setUserMap] = useState<Map<string, User>>(new Map());

    const upsertUsers = (newUsers: Map<string, User>) => {
        setUserMap(prev => {
            const next = new Map(prev);
            newUsers.forEach((v, k) => next.set(k, v));
            return next;
        });
    };

    return (
        <ChatContext.Provider value={{
            connection,
            isConnected,
            activeConversationId,
            setActiveConversationId,
            typingUsers,
            messages,
            addMessage,
            userMap,
            upsertUsers
        }}>
            {children}
        </ChatContext.Provider>
    );
};

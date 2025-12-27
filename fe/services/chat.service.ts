import { apiClient } from '@/lib/api';
import { Conversation, CreateConversationDto, Message, SendMessageDto } from '@/types/chat';

export const chatService = {
    getConversations: async (): Promise<Conversation[]> => {
        return apiClient.get<Conversation[]>('/api/chat/conversations');
    },

    createConversation: async (data: CreateConversationDto): Promise<Conversation> => {
        return apiClient.post<Conversation>('/api/chat/conversations', data);
    },

    getMessages: async (conversationId: string, beforeId?: string, limit: number = 20): Promise<Message[]> => {
        const query = new URLSearchParams({ limit: limit.toString() });
        if (beforeId) query.append('beforeId', beforeId);

        return apiClient.get<Message[]>(`/api/chat/conversations/${conversationId}/messages?${query.toString()}`);
    },

    sendMessage: async (data: SendMessageDto): Promise<Message> => {
        return apiClient.post<Message>('/api/chat/messages', data);
    },

    markAsRead: async (conversationId: string, messageId: string): Promise<void> => {
        return apiClient.post<void>(`/api/chat/conversations/${conversationId}/read?messageId=${messageId}`, {});
    }
};

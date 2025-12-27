export enum ConversationType {
    Private = 0,
    Group = 1
}

export enum MessageType {
    Text = 0,
    Image = 1
}

export interface ChatParticipant {
    userId: string;
    displayName?: string;
    avatarUrl?: string;
    // joinedAt, etc.
}

export interface Conversation {
    id: string;
    type: ConversationType;
    lastMessageAt: string; // ISO date
    lastMessageContent?: string;
    lastMessageSenderId?: string;
    lastMessageType: MessageType;
    participants: ChatParticipant[];
    unreadCount: number;
}

export interface Message {
    id: string;
    conversationId: string;
    senderId: string;
    content: string;
    type: MessageType;
    createdAt: string; // ISO date
}

export interface SendMessageDto {
    conversationId: string;
    content: string;
    type: MessageType;
}

export interface CreateConversationDto {
    type: ConversationType;
    participantIds: string[];
}

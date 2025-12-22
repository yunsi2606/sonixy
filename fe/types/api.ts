export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    userId: string;
    isEmailVerified: boolean;
}

export interface RegisterDto {
    email: string;
    username: string; // Add this
    password: string;
}

export interface LoginDto {
    email: string;
    password: string;
}

export interface User {
    id: string;
    username: string;
    firstName?: string;
    lastName?: string;
    displayName: string;
    email: string;
    bio: string;
    avatarUrl: string;
    createdAt: string;
    isEmailVerified?: boolean;
}

export interface Post {
    id: string;
    authorId: string;
    authorDisplayName: string;
    authorAvatarUrl: string;
    authorUsername: string;
    content: string;
    visibility: 'public' | 'followers';
    likeCount: number;
    isLiked?: boolean;
    media?: { type: 'image' | 'video', url: string }[];
    createdAt: string;
    updatedAt: string;
}

export interface CursorPage<T> {
    items: T[];
    nextCursor: string | null;
    hasMore: boolean;
}

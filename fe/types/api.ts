export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    userId: string;
}

export interface RegisterDto {
    email: string;
    password: string;
    displayName: string;
}

export interface LoginDto {
    email: string;
    password: string;
}

export interface User {
    id: string;
    displayName: string;
    email: string;
    bio: string;
    avatarUrl: string;
    createdAt: string;
}

export interface Post {
    id: string;
    authorId: string;
    content: string;
    visibility: 'public' | 'followers';
    likeCount: number;
    isLiked?: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface CursorPage<T> {
    items: T[];
    nextCursor: string | null;
    hasMore: boolean;
}

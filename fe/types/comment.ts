
export interface UserSimple {
    id: string;
    username: string;
    avatarUrl: string;
}

export interface Comment {
    id: string;
    content: string;
    author: UserSimple;
    createdAt: string;
    parentId: string | null; // null for ROOT, or ID of ROOT for REPLIES
    replyTo?: { // Metadata for who is being replied to (for visual @tagging)
        userId: string;
        username: string;
    };
    likes: number;
    isLiked?: boolean;
}

export interface CreateCommentDto {
    postId: string;
    content: string;
    authorUsername: string;
    authorAvatarUrl: string;
    parentId?: string;
    replyToUserId?: string;
    replyToUsername?: string;
}

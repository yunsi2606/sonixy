import React, { useEffect, useState } from 'react';
import { useChat } from '@/contexts/ChatContext';
import { chatService } from '@/services/chat.service';
import { Conversation, ConversationType } from '@/types/chat';
import { formatDistanceToNow } from 'date-fns';
import { socialService } from '@/services/social.service';
import { UserPlus } from 'lucide-react';
import { userService } from '@/services/user.service';
import { User } from '@/types/api';
import { authService } from '@/services/auth.service';
import { Avatar } from '@/components/ui/Avatar';

interface ConversationListProps {
    onSelect?: () => void;
    compact?: boolean;
    className?: string; // Enhancement: allow custom className
}

export const ConversationList: React.FC<ConversationListProps> = ({ onSelect, compact, className = "" }) => {
    const { activeConversationId, setActiveConversationId, typingUsers, userMap, upsertUsers, messages } = useChat();
    const [conversations, setConversations] = useState<Conversation[]>([]);
    const [mutualFollows, setMutualFollows] = useState<string[]>([]); // Store IDs of mutual follows
    const [isLoading, setIsLoading] = useState(true);
    const [currentUserId, setCurrentUserId] = useState<string | null>(null);
    const [newConversationId, setNewConversationId] = useState<string | null>(null);

    useEffect(() => {
        const loadCurrentUser = async () => {
            const user = await userService.getCurrentUser();
            if (user) setCurrentUserId(user.id);
        };
        loadCurrentUser();
    }, []);

    useEffect(() => {
        if (!currentUserId) return; // Wait for user ID

        const fetchData = async () => {
            try {
                const [convs, mutuals] = await Promise.all([
                    chatService.getConversations(),
                    socialService.getMutualFollows()
                ]);
                setConversations(convs);
                setMutualFollows(mutuals);

                // Collect all User IDs to fetch details
                const userIdsToFetch = new Set<string>();
                mutuals.forEach(id => userIdsToFetch.add(id));
                convs.forEach(c => c.participants.forEach(p => userIdsToFetch.add(p.userId)));

                if (userIdsToFetch.size > 0) {
                    const users = await userService.getUsersBatch(Array.from(userIdsToFetch));
                    const map = new Map<string, User>();
                    users.forEach(u => map.set(u.id, u));
                    upsertUsers(map);
                }

            } catch (error) {
                console.error("Failed to fetch chat data", error);
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, [currentUserId]);

    // Real-time: Listen for new messages in conversations we don't have yet
    useEffect(() => {
        if (!currentUserId || isLoading) return;

        messages.forEach((msgs, convId) => {
            const exists = conversations.some(c => c.id === convId);
            if (!exists) {
                // Fetch the new conversation
                chatService.getConversations().then(async (latestConvs) => {
                    const newConv = latestConvs.find(c => c.id === convId);
                    if (newConv) {
                        setConversations(prev => [newConv, ...prev]);
                        setNewConversationId(convId);
                        setTimeout(() => setNewConversationId(null), 3000); // Clear animation after 3s

                        // Also fetch users for this new conv
                        const userIdsToFetch = new Set<string>();
                        newConv.participants.forEach(p => userIdsToFetch.add(p.userId));
                        // Filter out known
                        const unknownIds = Array.from(userIdsToFetch).filter(id => !userMap.has(id));

                        if (unknownIds.length > 0) {
                            const newUsers = await userService.getUsersBatch(unknownIds);
                            const map = new Map<string, User>();
                            newUsers.forEach(u => map.set(u.id, u));
                            upsertUsers(map);
                        }
                    }
                });
            }
        });
    }, [messages, conversations, currentUserId, isLoading, userMap]);

    // Helper to start chat with mutual follow
    const handleStartChat = async (userId: string) => {
        try {
            const newConv = await chatService.createConversation({
                type: ConversationType.Private,
                participantIds: [userId]
            });
            // Update list if new
            if (!conversations.find(c => c.id === newConv.id)) {
                setConversations([newConv, ...conversations]);
            }
            setActiveConversationId(newConv.id);
            onSelect?.();
        } catch (error) {
            console.error("Failed to start chat", error);
            alert("Failed to start chat. Ensure you are mutual follows.");
        }
    };

    const getDisplayInfo = (conv: Conversation) => {
        if (conv.type === ConversationType.Group) {
            return { name: "Group Chat", avatar: undefined };
        }

        // Private Chat: Find the other participant
        const otherParticipant = conv.participants.find(p => p.userId !== currentUserId);
        if (otherParticipant) {
            const user = userMap.get(otherParticipant.userId);
            if (user) {
                return { name: user.displayName || user.username, avatar: user.avatarUrl };
            }
            // Fallback if user details not yet loaded (shouldn't happen often due to batch fetch)
            return { name: otherParticipant.displayName || "Chat User", avatar: otherParticipant.avatarUrl };
        }

        return { name: "Chat User", avatar: undefined };
    };

    const [showGroupModal, setShowGroupModal] = useState(false);
    const [selectedUsers, setSelectedUsers] = useState<Set<string>>(new Set());
    const [groupName, setGroupName] = useState("");

    const handleCreateGroup = async () => {
        if (selectedUsers.size < 1) {
            alert("Please select at least one person for the group.");
            return;
        }

        try {
            const newConv = await chatService.createConversation({
                type: ConversationType.Group,
                participantIds: Array.from(selectedUsers)
            });
            // Update list if new
            if (!conversations.find(c => c.id === newConv.id)) {
                setConversations([newConv, ...conversations]);
            }
            setActiveConversationId(newConv.id);
            setShowGroupModal(false);
            setSelectedUsers(new Set());
            setGroupName("");
            onSelect?.();
        } catch (error) {
            console.error("Failed to create group", error);
            alert("Failed to create group.");
        }
    };

    const toggleUserSelection = (uid: string) => {
        const newSet = new Set(selectedUsers);
        if (newSet.has(uid)) {
            newSet.delete(uid);
        } else {
            newSet.add(uid);
        }
        setSelectedUsers(newSet);
    };

    return (
        <div className={`flex flex-col h-full bg-transparent ${className} ${!className && !compact ? "w-80 border-r border-[var(--glass-border)]" : ""}`}>
            {!compact && (
                <div className="p-4 border-b border-[var(--glass-border)] flex justify-between items-center text-[var(--color-text-primary)]">
                    <span className="font-bold text-lg">Messages</span>
                    <button
                        onClick={() => setShowGroupModal(true)}
                        className="text-xs bg-[var(--color-primary)] px-2 py-1 rounded hover:opacity-90"
                    >
                        + Group
                    </button>
                </div>
            )}

            {showGroupModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
                    <div className="bg-[var(--color-bg-card)] border border-[var(--glass-border)] rounded-xl w-full max-w-sm p-4 shadow-2xl">
                        <h3 className="text-lg font-bold mb-4 text-[var(--color-text-primary)]">New Group Chat</h3>

                        <div className="mb-4">
                            <label className="text-xs text-[var(--color-text-muted)] block mb-2">Select Members (Mutual Follows)</label>
                            <div className="max-h-48 overflow-y-auto space-y-2 custom-scrollbar border border-[var(--glass-border)] rounded p-2">
                                {mutualFollows.map(uid => {
                                    const user = userMap.get(uid);
                                    return (
                                        <div
                                            key={uid}
                                            onClick={() => toggleUserSelection(uid)}
                                            className={`flex items-center gap-2 p-2 rounded cursor-pointer ${selectedUsers.has(uid) ? 'bg-[var(--color-primary)]/20 border border-[var(--color-primary)]' : 'hover:bg-white/5'}`}
                                        >
                                            <div className={`w-4 h-4 rounded-full border flex items-center justify-center ${selectedUsers.has(uid) ? 'bg-[var(--color-primary)] border-[var(--color-primary)]' : 'border-[var(--color-text-muted)]'}`}>
                                                {selectedUsers.has(uid) && <span className="text-[10px] text-white">✓</span>}
                                            </div>
                                            {user?.avatarUrl ? (
                                                <img src={user.avatarUrl} className="w-6 h-6 rounded-full object-cover" alt={user.username} />
                                            ) : (
                                                <div className="w-6 h-6 rounded-full bg-[var(--color-surface)]" />
                                            )}
                                            <span className="text-sm text-[var(--color-text-primary)] truncate">{user?.displayName || "User"}</span>
                                        </div>
                                    );
                                })}
                                {mutualFollows.length === 0 && <p className="text-xs text-[var(--color-text-muted)]">No mutual follows found.</p>}
                            </div>
                        </div>

                        <div className="flex justify-end gap-2 mt-4">
                            <button
                                onClick={() => setShowGroupModal(false)}
                                className="px-3 py-1.5 rounded text-sm text-[var(--color-text-primary)] hover:bg-white/10"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleCreateGroup}
                                disabled={selectedUsers.size < 1}
                                className="px-3 py-1.5 rounded text-sm bg-[var(--color-primary)] text-white hover:opacity-90 disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                Create Group ({selectedUsers.size})
                            </button>
                        </div>
                    </div>
                </div>
            )}

            <div className="flex-1 overflow-y-auto custom-scrollbar">
                {isLoading && (
                    <div className="p-4 space-y-4">
                        {[1, 2, 3].map(i => (
                            <div key={i} className="flex gap-3 animate-pulse">
                                <div className="w-10 h-10 rounded-full bg-white/10" />
                                <div className="flex-1 space-y-2 py-1">
                                    <div className="h-3 bg-white/10 rounded w-1/3" />
                                    <div className="h-2 bg-white/10 rounded w-1/2" />
                                </div>
                            </div>
                        ))}
                    </div>
                )}

                {/* Contacts (Mutual Follows) - Horizontal List */}
                {mutualFollows.length > 0 && !compact && (
                    <div className="p-3 border-b border-[var(--glass-border)] bg-white/5">
                        <h3 className="text-xs font-bold text-[var(--color-text-muted)] mb-2 uppercase">Contacts</h3>
                        <div className="flex gap-2 overflow-x-auto pb-2 scrollbar-hide">
                            {mutualFollows.map(uid => {
                                const user = userMap.get(uid);
                                return (
                                    <div
                                        key={uid}
                                        onClick={() => handleStartChat(uid)}
                                        className="flex flex-col items-center gap-1 cursor-pointer min-w-[60px]"
                                    >
                                        <div className="w-10 h-10 rounded-full bg-[var(--color-surface)] border border-[var(--glass-border)] flex items-center justify-center text-[var(--color-text-secondary)] hover:border-[var(--color-primary)] transition-colors overflow-hidden">
                                            {user?.avatarUrl ? (
                                                <img src={user.avatarUrl} className="w-full h-full object-cover" alt={user.username} />
                                            ) : (
                                                <UserPlus size={18} />
                                            )}
                                        </div>
                                        <span className="text-[10px] text-[var(--color-text-primary)] truncate w-full text-center">{user?.displayName?.split(' ')[0] || "User"}</span>
                                    </div>
                                );
                            })}
                        </div>
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
                                ${newConversationId === conv.id ? "animate-pulse bg-green-500/10" : ""}
                            `}
                        >
                            {/* Avatar */}
                            <div className="relative shrink-0">
                                <Avatar
                                    src={avatar}
                                    alt={name}
                                    className="p-0.5" // preserves the padding for the ring effect if needed, though Avatar has its own ring logic.
                                    // The original had a gradient border: bg-gradient-to-br from-[var(--color-primary)] to-[var(--color-secondary)] p-0.5
                                    // Our new Avatar component handles rings beautifully.
                                    // Let's rely on standard Avatar, but if we want the exact gradient style of THIS component which used 'to-br', 
                                    // whereas ui/Avatar uses 'to-tr', it's fine to standardize on 'to-tr' which is in ui/Avatar.
                                    hasRing={true}
                                    size="md"
                                />
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
            </div >
        </div >
    );
};

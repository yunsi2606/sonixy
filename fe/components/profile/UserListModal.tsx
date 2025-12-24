'use client';

import React, { useState, useEffect } from 'react';
import type { User } from '@/types/api';
import { socialService } from '@/services/social.service';
import { userService } from '@/services/user.service';
import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';

interface UserListModalProps {
    isOpen: boolean;
    onClose: () => void;
    initialType: 'followers' | 'following';
    userId: string;
}

export function UserListModal({ isOpen, onClose, initialType, userId }: UserListModalProps) {
    const [type, setType] = useState(initialType);
    const [users, setUsers] = useState<User[]>([]);
    const [isLoading, setIsLoading] = useState(false);

    // Pagination state
    const [hasMore, setHasMore] = useState(true);
    const [skip, setSkip] = useState(0);
    const LIMIT = 20;

    const { user: currentUser } = useAuth();

    useEffect(() => {
        if (isOpen) {
            setType(initialType); // Reset type to what was clicked
            resetAndLoad(initialType);
        }
    }, [isOpen]);

    // Handle tab switching inside modal
    const handleTabChange = (newType: 'followers' | 'following') => {
        if (newType === type) return;
        setType(newType);
        resetAndLoad(newType);
    };

    const resetAndLoad = (currentType: 'followers' | 'following') => {
        setUsers([]);
        setHasMore(true);
        setSkip(0);
        loadUsers(currentType, 0);
    };

    const loadUsers = async (currentType: 'followers' | 'following', currentSkip: number) => {
        if (isLoading) return;

        setIsLoading(true);
        try {
            // 1. Fetch IDs from Social Service
            let ids: string[] = [];
            if (currentType === 'followers') {
                ids = await socialService.getFollowers(userId, currentSkip, LIMIT);
            } else {
                ids = await socialService.getFollowing(userId, currentSkip, LIMIT);
            }

            if (ids.length < LIMIT) {
                setHasMore(false);
            }

            if (ids.length === 0) {
                setIsLoading(false);
                return;
            }

            // 2. Fetch User Details from User Service
            const userDetails = await userService.getUsersBatch(ids);

            setUsers(prev => currentSkip === 0 ? userDetails : [...prev, ...userDetails]);
            setSkip(currentSkip + ids.length);
        } catch (error) {
            console.error('Failed to load users', error);
        } finally {
            setIsLoading(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-fade-in" onClick={onClose}>
            <div
                className="w-full max-w-md bg-[var(--color-surface)] border border-[var(--color-border)] rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[80vh]"
                onClick={e => e.stopPropagation()}
            >
                {/* Header / Tabs */}
                <div className="flex border-b border-[var(--color-border)]">
                    <button
                        onClick={() => handleTabChange('followers')}
                        className={`flex-1 py-4 font-bold text-center transition-colors ${type === 'followers'
                                ? 'text-[var(--color-primary)] border-b-2 border-[var(--color-primary)]'
                                : 'text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)]'
                            }`}
                    >
                        Followers
                    </button>
                    <button
                        onClick={() => handleTabChange('following')}
                        className={`flex-1 py-4 font-bold text-center transition-colors ${type === 'following'
                                ? 'text-[var(--color-primary)] border-b-2 border-[var(--color-primary)]'
                                : 'text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)]'
                            }`}
                    >
                        Following
                    </button>
                </div>

                {/* List */}
                <div className="flex-1 overflow-y-auto p-4 space-y-4 min-h-[300px]" onScroll={(e) => {
                    const { scrollTop, clientHeight, scrollHeight } = e.currentTarget;
                    if (scrollHeight - scrollTop <= clientHeight + 50 && hasMore && !isLoading) {
                        loadUsers(type, skip);
                    }
                }}>
                    {users.map(u => (
                        <div key={u.id} className="flex items-center gap-3">
                            <Link href={`/u/${u.username}`} onClick={onClose} className="flex-shrink-0">
                                <div className="w-10 h-10 rounded-full border border-[var(--color-border)] bg-[var(--color-bg-deep)] overflow-hidden">
                                    {u.avatarUrl ? (
                                        <img src={u.avatarUrl} alt={u.displayName} className="w-full h-full object-cover" />
                                    ) : (
                                        <div className="w-full h-full flex items-center justify-center bg-gray-700 text-xs font-bold text-white">
                                            {u.firstName?.[0]}{u.lastName?.[0]}
                                        </div>
                                    )}
                                </div>
                            </Link>
                            <div className="flex-1 min-w-0">
                                <Link href={`/u/${u.username}`} onClick={onClose} className="font-bold text-[var(--color-text-primary)] hover:underline truncate block">
                                    {u.displayName}
                                </Link>
                                <div className="text-xs text-[var(--color-text-secondary)] truncate">@{u.username}</div>
                            </div>
                            {/* Logic to show Follow/Unfollow button could go here, omitting for simplicity to keep generic */}
                        </div>
                    ))}

                    {/* Loading State */}
                    {isLoading && (
                        <div className="flex justify-center p-4">
                            <div className="w-6 h-6 border-2 border-[var(--color-primary)] border-t-transparent rounded-full animate-spin" />
                        </div>
                    )}

                    {/* Empty State */}
                    {!isLoading && users.length === 0 && (
                        <div className="text-center py-12 text-[var(--color-text-muted)]">
                            No {type} yet.
                        </div>
                    )}
                </div>

                {/* Footer handling for close button if needed, but backdrop click works */}
            </div>
        </div>
    );
}

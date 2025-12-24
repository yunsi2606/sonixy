'use client';

import { useState, useEffect } from 'react';
import type { User } from '@/types/api';
import { socialService } from '@/services/social.service';
import { UserListModal } from './UserListModal';

interface ProfileHeaderProps {
    user: User;
    isOwnProfile: boolean;
    onEdit?: () => void;
}

export function ProfileHeader({ user, isOwnProfile, onEdit }: ProfileHeaderProps) {
    const [stats, setStats] = useState({ followers: 0, following: 0, posts: 0 });
    const [isFollowing, setIsFollowing] = useState(false);
    const [isLoadingStats, setIsLoadingStats] = useState(true);

    const [modalConfig, setModalConfig] = useState<{ isOpen: boolean; type: 'followers' | 'following' }>({
        isOpen: false,
        type: 'followers'
    });

    useEffect(() => {
        loadStats();
    }, [user.id]);

    const loadStats = async () => {
        try {
            const [followers, following, followingStatus] = await Promise.all([
                socialService.getFollowersCount(user.id),
                socialService.getFollowingCount(user.id),
                !isOwnProfile ? socialService.getFollowStatus(user.id) : Promise.resolve(false)
            ]);

            setStats(prev => ({ ...prev, followers, following }));
            if (!isOwnProfile) setIsFollowing(followingStatus);
        } catch (err) {
            console.error('Failed to load stats', err);
        } finally {
            setIsLoadingStats(false);
        }
    };

    const handleFollowToggle = async () => {
        try {
            if (isFollowing) {
                await socialService.unfollowUser(user.id);
                setStats(prev => ({ ...prev, followers: Math.max(0, prev.followers - 1) }));
            } else {
                await socialService.followUser(user.id);
                setStats(prev => ({ ...prev, followers: prev.followers + 1 }));
            }
            setIsFollowing(!isFollowing);
        } catch (err) {
            console.error('Failed to toggle follow', err);
        }
    };

    const openModal = (type: 'followers' | 'following') => {
        setModalConfig({ isOpen: true, type });
    };

    return (
        <div className="glass rounded-2xl overflow-hidden mb-8 animate-fade-in">
            {/* Cover Image */}
            <div className="h-48 bg-gradient-to-r from-[var(--color-primary)] to-[var(--color-secondary)] opacity-80" />

            <div className="px-8 pb-8">
                <div className="flex justify-between items-end -mt-12 mb-6">
                    {/* Avatar */}
                    <div className="relative">
                        <div className="w-32 h-32 rounded-full border-4 border-[var(--color-bg-primary)] overflow-hidden bg-[var(--color-surface)]">
                            {user.avatarUrl ? (
                                <img src={user.avatarUrl} alt={user.displayName} className="w-full h-full object-cover" />
                            ) : (
                                <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-gray-700 to-gray-900 text-3xl font-bold text-white">
                                    {user.firstName?.[0]}{user.lastName?.[0]}
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Action Button */}
                    <div className="mb-4">
                        {isOwnProfile ? (
                            <button
                                onClick={onEdit}
                                className="px-6 py-2 bg-[var(--color-surface)] hover:bg-[var(--color-surface-hover)] border border-[var(--color-border)] rounded-xl font-medium transition-all hover:scale-105 active:scale-95"
                            >
                                Edit Profile
                            </button>
                        ) : (
                            <button
                                onClick={handleFollowToggle}
                                className={`px-8 py-2 rounded-xl font-medium transition-all hover:scale-105 active:scale-95 ${isFollowing
                                    ? 'bg-[var(--color-surface)] text-[var(--color-text-primary)] border border-[var(--color-border)]'
                                    : 'bg-[var(--color-primary)] text-white hover:bg-[var(--color-primary-hover)]'
                                    }`}
                            >
                                {isFollowing ? 'Following' : 'Follow'}
                            </button>
                        )}
                    </div>
                </div>

                {/* Info */}
                <div className="mb-6">
                    <h1 className="text-3xl font-bold mb-1">{user.displayName}</h1>
                    <p className="text-[var(--color-text-secondary)] mb-4">{user.email}</p>
                    {user.bio && (
                        <p className="text-[var(--color-text-primary)] max-w-2xl leading-relaxed">
                            {user.bio}
                        </p>
                    )}
                </div>

                {/* Stats */}
                <div className="flex gap-8 border-t border-[var(--color-border)] pt-6">
                    <div className="text-center cursor-pointer hover:opacity-80 transition-opacity">
                        <div className="text-2xl font-bold">{stats.posts}</div>
                        <div className="text-sm text-[var(--color-text-secondary)]">Posts</div>
                    </div>
                    <div
                        className="text-center cursor-pointer hover:opacity-80 transition-opacity"
                        onClick={() => openModal('followers')}
                    >
                        <div className="text-2xl font-bold">{stats.followers}</div>
                        <div className="text-sm text-[var(--color-text-secondary)]">Followers</div>
                    </div>
                    <div
                        className="text-center cursor-pointer hover:opacity-80 transition-opacity"
                        onClick={() => openModal('following')}
                    >
                        <div className="text-2xl font-bold">{stats.following}</div>
                        <div className="text-sm text-[var(--color-text-secondary)]">Following</div>
                    </div>
                </div>
            </div>

            {/* User List Modal */}
            <UserListModal
                isOpen={modalConfig.isOpen}
                onClose={() => setModalConfig(prev => ({ ...prev, isOpen: false }))}
                initialType={modalConfig.type}
                userId={user.id}
            />
        </div>
    );
}

'use client';

import { useState, useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';
import { userService } from '@/services/user.service';
import { postService } from '@/services/post.service';
import { useInfiniteScroll } from '@/hooks/useInfiniteScroll';
import { PostCard } from '@/components/common/PostCard';
import { ProfileHeader } from '@/components/profile/ProfileHeader';
import { ProfileSkeleton } from '@/components/skeletons/ProfileSkeleton';
import { PostSkeleton } from '@/components/skeletons/PostSkeleton';
import type { User } from '@/types/api';

export default function UserProfilePage() {
    const router = useRouter();
    const params = useParams();
    const username = params?.username as string;

    const { isAuthenticated, userId: currentUserId, user: currentUser, logout } = useAuth();
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isEditing, setIsEditing] = useState(false);
    const [editForm, setEditForm] = useState({ firstName: '', lastName: '', bio: '' });
    const [isSaving, setIsSaving] = useState(false);

    const { items: posts, isLoading: postsLoading, refresh } = useInfiniteScroll({
        fetchFunction: (cursor) => {
            return postService.getUserPosts(username, cursor);
        },
        pageSize: 20,
        dependencies: [username], // Refresh if username changes
        enabled: !!username
    });

    useEffect(() => {
        loadUser();
    }, [username]);

    const isOwnProfile = user?.id === currentUserId;

    // Initialize edit form when user loads and we might edit
    useEffect(() => {
        if (user && isOwnProfile) {
            setEditForm({
                firstName: user.firstName || '',
                lastName: user.lastName || '',
                bio: user.bio,
            });
        }
    }, [user, isOwnProfile]);

    const loadUser = async () => {
        console.log('loadUser called with username:', username);
        if (!username) {
            console.log('No username, stopping loading');
            setIsLoading(false);
            return;
        }

        setIsLoading(true);
        try {
            console.log('Fetching user data for:', username);
            const userData = await userService.getUser(username);
            console.log('User data received:', userData);
            setUser(userData);
        } catch (error) {
            console.error('Failed to load user:', error);
            // Redirect or show 404
        } finally {
            setIsLoading(false);
        }
    };

    const handleSave = async () => {
        if (!user) return;

        setIsSaving(true);
        try {
            const updated = await userService.updateUser(user.id, editForm);
            setUser(updated);
            setIsEditing(false);
        } catch (error) {
            console.error('Failed to update profile:', error);
        } finally {
            setIsSaving(false);
        }
    };

    if (isLoading) {
        return (
            <div className="min-h-screen bg-[var(--color-bg-primary)] pb-8 pt-4 max-w-4xl mx-auto px-4">
                <ProfileSkeleton />
            </div>
        );
    }

    if (!user) {
        return (
            <div className="min-h-screen bg-[var(--color-bg-primary)] flex items-center justify-center">
                <div className="text-[var(--color-text-secondary)]">User not found</div>
            </div>
        );
    }


    return (
        <div className="pb-8">
            {/* Header */}
            <header className="glass sticky top-0 z-10 border-b border-[var(--color-border)]">
                <div className="max-w-4xl mx-auto px-4 py-4 flex items-center justify-between">
                    <button
                        onClick={() => router.push('/feed')}
                        className="text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] transition-colors"
                    >
                        ← Back to Feed
                    </button>
                    <div className="flex gap-4">
                        {isAuthenticated && !isOwnProfile && (
                            <button
                                onClick={() => router.push(`/u/${currentUser?.username}`)}
                                className="text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] transition-colors"
                            >
                                My Profile
                            </button>
                        )}
                        {isOwnProfile && (
                            <button
                                onClick={() => logout()}
                                className="px-4 py-2 text-sm text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] transition-colors"
                            >
                                Logout
                            </button>
                        )}
                    </div>
                </div>
            </header>

            <div className="max-w-4xl mx-auto px-4 py-8">
                {isEditing && isOwnProfile ? (
                    <div className="glass p-8 rounded-2xl mb-8 animate-fade-in">
                        <h2 className="text-2xl font-bold mb-6">Edit Profile</h2>
                        <div className="space-y-4 max-w-xl">
                            <div className="grid grid-cols-2 gap-4">
                                <input
                                    type="text"
                                    value={editForm.firstName}
                                    onChange={(e) => setEditForm({ ...editForm, firstName: e.target.value })}
                                    className="w-full px-4 py-2 bg-[var(--color-surface)] border border-[var(--color-border)] rounded-xl focus:outline-none focus:border-[var(--color-border-focus)]"
                                    placeholder="First Name"
                                />
                                <input
                                    type="text"
                                    value={editForm.lastName}
                                    onChange={(e) => setEditForm({ ...editForm, lastName: e.target.value })}
                                    className="w-full px-4 py-2 bg-[var(--color-surface)] border border-[var(--color-border)] rounded-xl focus:outline-none focus:border-[var(--color-border-focus)]"
                                    placeholder="Last Name"
                                />
                            </div>
                            <textarea
                                value={editForm.bio}
                                onChange={(e) => setEditForm({ ...editForm, bio: e.target.value })}
                                className="w-full px-4 py-2 bg-[var(--color-surface)] border border-[var(--color-border)] rounded-xl focus:outline-none focus:border-[var(--color-border-focus)] resize-none"
                                placeholder="Bio"
                                rows={3}
                            />
                            <div className="flex gap-3 pt-4">
                                <button
                                    onClick={handleSave}
                                    disabled={isSaving}
                                    className="px-6 py-2 bg-[var(--color-primary)] hover:bg-[var(--color-primary-hover)] text-white rounded-xl font-medium transition-all disabled:opacity-50"
                                >
                                    {isSaving ? 'Saving...' : 'Save Changes'}
                                </button>
                                <button
                                    onClick={() => {
                                        setIsEditing(false);
                                        // Reset form
                                        if (user) {
                                            setEditForm({
                                                firstName: user.firstName || '',
                                                lastName: user.lastName || '',
                                                bio: user.bio,
                                            });
                                        }
                                    }}
                                    className="px-6 py-2 bg-[var(--color-surface)] hover:bg-[var(--color-surface-hover)] rounded-xl font-medium transition-all"
                                >
                                    Cancel
                                </button>
                            </div>
                        </div>
                    </div>
                ) : (
                    <ProfileHeader
                        user={user}
                        isOwnProfile={isOwnProfile}
                        onEdit={() => setIsEditing(true)}
                    />
                )}

                {/* Posts Section */}
                <div>
                    <h2 className="text-2xl font-bold mb-6">{isOwnProfile ? 'Your Posts' : 'Posts'}</h2>
                    <div className="space-y-4">
                        {postsLoading && posts.length === 0 ? (
                            <div className="space-y-4">
                                <PostSkeleton />
                                <PostSkeleton />
                            </div>
                        ) : posts.length === 0 ? (
                            <div className="glass p-12 rounded-2xl text-center">
                                <p className="text-[var(--color-text-secondary)] mb-4">No posts yet</p>
                                {isOwnProfile && (
                                    <button
                                        onClick={() => router.push('/feed')}
                                        className="px-6 py-3 bg-[var(--color-primary)] hover:bg-[var(--color-primary-hover)] text-white rounded-xl font-medium transition-all"
                                    >
                                        Create your first post
                                    </button>
                                )}
                            </div>
                        ) : (
                            posts.map((post) => <PostCard key={post.id} post={post} />)
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

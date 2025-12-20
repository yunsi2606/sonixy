'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';
import { userService, type UpdateUserDto } from '@/services/user.service';
import { postService } from '@/services/post.service';
import { useInfiniteScroll } from '@/hooks/useInfiniteScroll';
import { PostCard } from '@/components/common/PostCard';
import { ProfileHeader } from '@/components/profile/ProfileHeader';
import type { User } from '@/types/api';

export default function ProfilePage() {
    const router = useRouter();
    const { isAuthenticated, userId, logout, isLoading: authLoading } = useAuth();
    const [user, setUser] = useState<User | null>(null);
    const [isEditing, setIsEditing] = useState(false);
    const [editForm, setEditForm] = useState({ firstName: '', lastName: '', bio: '' });
    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);

    const { items: posts, isLoading: postsLoading } = useInfiniteScroll({
        fetchFunction: (cursor) => {
            if (!userId) throw new Error('Not authenticated');
            return postService.getUserPosts(userId, cursor);
        },
        pageSize: 20,
    });

    useEffect(() => {
        if (authLoading) return;

        if (!isAuthenticated) {
            router.push('/login');
            return;
        }

        loadUser();
    }, [isAuthenticated, authLoading, router]);

    const loadUser = async () => {
        try {
            const userData = await userService.getCurrentUser();
            setUser(userData);
            setEditForm({
                firstName: userData.firstName || '',
                lastName: userData.lastName || '',
                bio: userData.bio,
            });
        } catch (error) {
            console.error('Failed to load user:', error);
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
            <div className="min-h-screen bg-[var(--color-bg-primary)] flex items-center justify-center">
                <div className="text-[var(--color-text-secondary)]">Loading profile...</div>
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
        <main className="min-h-screen bg-[var(--color-bg-primary)]">
            {/* Header */}
            <header className="glass sticky top-0 z-10 border-b border-[var(--color-border)]">
                <div className="max-w-4xl mx-auto px-4 py-4 flex items-center justify-between">
                    <button
                        onClick={() => router.push('/feed')}
                        className="text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] transition-colors"
                    >
                        ← Back to Feed
                    </button>
                    <button
                        onClick={() => logout()}
                        className="px-4 py-2 text-sm text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] transition-colors"
                    >
                        Logout
                    </button>
                </div>
            </header>

            <div className="max-w-4xl mx-auto px-4 py-8">
                {/* Profile Section */}
                {isEditing ? (
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
                                        setEditForm({
                                            firstName: user.firstName || '',
                                            lastName: user.lastName || '',
                                            bio: user.bio,
                                        });
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
                        isOwnProfile={true}
                        onEdit={() => setIsEditing(true)}
                    />
                )}

                {/* Posts Section */}
                <div>
                    <h2 className="text-2xl font-bold mb-6">Your Posts</h2>
                    <div className="space-y-4">
                        {postsLoading && posts.length === 0 ? (
                            <div className="glass p-8 rounded-2xl text-center text-[var(--color-text-secondary)]">
                                Loading posts...
                            </div>
                        ) : posts.length === 0 ? (
                            <div className="glass p-12 rounded-2xl text-center">
                                <p className="text-[var(--color-text-secondary)] mb-4">No posts yet</p>
                                <button
                                    onClick={() => router.push('/feed')}
                                    className="px-6 py-3 bg-[var(--color-primary)] hover:bg-[var(--color-primary-hover)] text-white rounded-xl font-medium transition-all"
                                >
                                    Create your first post
                                </button>
                            </div>
                        ) : (
                            posts.map((post) => <PostCard key={post.id} post={post} />)
                        )}
                    </div>
                </div>
            </div>
        </main>
    );
}

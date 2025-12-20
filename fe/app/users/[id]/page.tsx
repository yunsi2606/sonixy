'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';
import { userService } from '@/services/user.service';
import { postService } from '@/services/post.service';
import { useInfiniteScroll } from '@/hooks/useInfiniteScroll';
import { PostCard } from '@/components/common/PostCard';
import { ProfileHeader } from '@/components/profile/ProfileHeader';
import type { User } from '@/types/api';

export default function UserProfilePage({ params }: { params: { id: string } }) {
    const router = useRouter();
    const { isAuthenticated, userId: currentUserId } = useAuth();
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const { items: posts, isLoading: postsLoading } = useInfiniteScroll({
        fetchFunction: (cursor) => {
            return postService.getUserPosts(params.id, cursor);
        },
        pageSize: 20,
    });

    useEffect(() => {
        loadUser();
    }, [params.id]);

    const loadUser = async () => {
        try {
            const userData = await userService.getUser(params.id);
            setUser(userData);
        } catch (error) {
            console.error('Failed to load user:', error);
            // Redirect to feed if user not found (or show 404)
            // router.push('/feed');
        } finally {
            setIsLoading(false);
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

    const isOwnProfile = currentUserId === user.id;

    // If viewing own profile via /users/:id, consider redirecting to /profile 
    // or just render it as is. Rendering "as is" is fine.

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
                    {isAuthenticated && (
                        <button
                            onClick={() => router.push('/profile')}
                            className="text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] transition-colors"
                        >
                            My Profile
                        </button>
                    )}
                </div>
            </header>

            <div className="max-w-4xl mx-auto px-4 py-8">
                <ProfileHeader
                    user={user}
                    isOwnProfile={isOwnProfile}
                    onEdit={() => router.push('/profile')} // Redirect to main profile if they click edit on their own public profile
                />

                {/* Posts Section */}
                <div>
                    <h2 className="text-2xl font-bold mb-6">Posts</h2>
                    <div className="space-y-4">
                        {postsLoading && posts.length === 0 ? (
                            <div className="glass p-8 rounded-2xl text-center text-[var(--color-text-secondary)]">
                                Loading posts...
                            </div>
                        ) : posts.length === 0 ? (
                            <div className="glass p-12 rounded-2xl text-center">
                                <p className="text-[var(--color-text-secondary)] mb-4">No posts yet</p>
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

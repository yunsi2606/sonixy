'use client';

import { useState, useEffect, useCallback } from 'react';
import { searchUsers, searchPosts, searchAll, UserSearchResult, PostSearchResult } from '@/services/search.service';
import { Avatar } from '@/components/ui/Avatar';
import Link from 'next/link';
import { Search } from 'lucide-react';
import { PostCard } from '@/components/common/PostCard';
import type { Post } from '@/types/api';

type Tab = 'all' | 'people' | 'posts';

export default function SearchPage() {
    const [query, setQuery] = useState('');
    const [activeTab, setActiveTab] = useState<Tab>('all');
    const [users, setUsers] = useState<UserSearchResult[]>([]);
    const [posts, setPosts] = useState<PostSearchResult[]>([]);
    const [loading, setLoading] = useState(false);
    const [searched, setSearched] = useState(false);

    const handleSearch = useCallback(async (searchQuery: string) => {
        if (!searchQuery.trim()) {
            setUsers([]);
            setPosts([]);
            setSearched(false);
            return;
        }

        setLoading(true);
        setSearched(true);

        try {
            if (activeTab === 'all') {
                const results = await searchAll(searchQuery);
                setUsers(results.users || []);
                setPosts(results.posts || []);
            } else if (activeTab === 'people') {
                const results = await searchUsers(searchQuery);
                setUsers(results);
                setPosts([]);
            } else {
                const results = await searchPosts(searchQuery);
                setPosts(results);
                setUsers([]);
            }
        } catch (error) {
            console.error('Search failed:', error);
        } finally {
            setLoading(false);
        }
    }, [activeTab]);

    // Debounced search
    useEffect(() => {
        const timeoutId = setTimeout(() => {
            if (query) {
                handleSearch(query);
            }
        }, 300);

        return () => clearTimeout(timeoutId);
    }, [query, handleSearch]);

    // Re-search when tab changes
    useEffect(() => {
        if (query) {
            handleSearch(query);
        }
    }, [activeTab]);

    return (
        <div className="w-full max-w-3xl mx-auto">
            {/* Header & Search Bar */}
            <div className="glass-base rounded-2xl p-6 mb-8 flex flex-col gap-6">
                <h1 className="text-2xl font-bold text-white">Search</h1>
                <div className="relative group">
                    <input
                        type="text"
                        placeholder="Search people or posts..."
                        value={query}
                        onChange={(e) => setQuery(e.target.value)}
                        className="w-full bg-white/5 border border-[var(--glass-border)] rounded-xl px-12 py-4 text-sm text-white placeholder-white/30 focus:border-[var(--color-primary)] focus:bg-white/10 outline-none transition-all shadow-[var(--shadow-soft)]"
                    />
                    <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-[var(--color-primary)] transition-colors" size={20} />
                </div>
            </div>

            {/* Tabs */}
            <div className="flex gap-4 mb-8">
                {(['all', 'people', 'posts'] as const).map(tab => (
                    <button
                        key={tab}
                        onClick={() => setActiveTab(tab)}
                        className={`flex-1 py-3 px-6 rounded-xl font-medium transition-all ${activeTab === tab 
                            ? 'bg-gradient-to-r from-[var(--color-primary)] to-[var(--color-secondary)] text-white shadow-[var(--shadow-neon)] border border-transparent'
                            : 'bg-white/5 border border-[var(--glass-border)] text-[var(--color-text-secondary)] hover:bg-white/10 hover:text-white'
                        }`}
                    >
                        {tab.charAt(0).toUpperCase() + tab.slice(1)}
                    </button>
                ))}
            </div>

            {/* Results */}
            <div className="flex flex-col gap-4">
                {loading ? (
                    <div className="flex justify-center py-12">
                        <div className="w-8 h-8 rounded-full border-2 border-[var(--color-primary)] border-t-transparent animate-spin"></div>
                    </div>
                ) : (
                    <>
                        {/* Users Section */}
                        {(activeTab === 'all' || activeTab === 'people') && users.length > 0 && (
                            <div className="mb-6">
                                {activeTab === 'all' && <h2 className="text-lg font-bold text-white mb-4">People</h2>}
                                <div className="space-y-3">
                                    {users.map((user) => (
                                        <Link
                                            key={user.id}
                                            href={`/u/${user.username}`}
                                            className="flex items-center gap-4 p-4 glass-base rounded-2xl border border-[var(--glass-border)] hover:bg-white/10 transition-colors group"
                                        >
                                            <Avatar src={user.avatarUrl} alt={user.displayName} size="md" />
                                            <div className="flex-1 min-w-0">
                                                <p className="font-semibold text-white truncate group-hover:text-[var(--color-primary)] transition-colors">{user.displayName}</p>
                                                <p className="text-sm text-[var(--color-text-muted)]">@{user.username}</p>
                                                {user.bio && <p className="text-sm text-[var(--color-text-secondary)] truncate mt-1">{user.bio}</p>}
                                            </div>
                                        </Link>
                                    ))}
                                </div>
                            </div>
                        )}

                        {/* Posts Section */}
                        {(activeTab === 'all' || activeTab === 'posts') && posts.length > 0 && (
                            <div>
                                {activeTab === 'all' && <h2 className="text-lg font-bold text-white mb-4">Posts</h2>}
                                <div className="space-y-4">
                                    {posts.map((post) => {
                                        // Mapped to satisfy PostCard props
                                        const mappedPost: Post = {
                                            id: post.id,
                                            authorId: post.authorId,
                                            authorDisplayName: post.authorDisplayName,
                                            authorUsername: post.authorUsername,
                                            authorAvatarUrl: post.authorAvatarUrl,
                                            content: post.content,
                                            hashtags: post.hashtags,
                                            visibility: 'public', // Search results are public
                                            likeCount: 0,
                                            isLiked: false,
                                            createdAt: post.createdAt,
                                        };
                                        return <PostCard key={post.id} post={mappedPost} variant="default" />;
                                    })}
                                </div>
                            </div>
                        )}

                        {/* Empty State */}
                        {searched && !loading && users.length === 0 && posts.length === 0 && (
                            <div className="text-center py-16 text-[var(--color-text-muted)] glass-base rounded-2xl border border-dashed border-[var(--glass-border)]">
                                <div className="text-4xl mb-4">🔍</div>
                                <h3 className="text-lg font-bold text-white mb-2">No results found</h3>
                                <p>We couldn't find anything matching "{query}"</p>
                            </div>
                        )}

                        {/* Initial State */}
                        {!searched && !query && (
                            <div className="text-center py-16 text-[var(--color-text-muted)] glass-base rounded-2xl border border-dashed border-[var(--glass-border)]">
                                <Search size={48} className="mx-auto mb-4 opacity-20" />
                                <h3 className="text-lg font-bold text-white mb-2">Search Sonixy</h3>
                                <p>Type to search for people or posts</p>
                            </div>
                        )}
                    </>
                )}
            </div>
        </div>
    );
}

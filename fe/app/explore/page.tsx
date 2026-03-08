'use client';

import { useState, useEffect } from 'react';
import { getTrendingHashtags, TrendingHashtag } from '@/services/search.service';
import { Avatar } from '@/components/ui/Avatar';
import Link from 'next/link';

interface SuggestedUser {
    id: string;
    username: string;
    displayName: string;
    avatarUrl: string;
}

// Note: For suggested users, we'll use the social service endpoint
// and then fetch user details via batch

export default function ExplorePage() {
    const [trendingHashtags, setTrendingHashtags] = useState<TrendingHashtag[]>([]);
    const [suggestedUsers, setSuggestedUsers] = useState<SuggestedUser[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchExploreData = async () => {
            try {
                // Fetch trending hashtags
                const hashtags = await getTrendingHashtags(10);
                setTrendingHashtags(hashtags);

                // For suggested users, we'd need to implement the endpoint
                // For now, just show trending hashtags
            } catch (error) {
                console.error('Failed to fetch explore data:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchExploreData();
    }, []);

    if (loading) {
        return (
            <div className="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900">
            <div className="max-w-2xl mx-auto px-4 py-6">
                {/* Header */}
                <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">Explore</h1>

                {/* Trending Hashtags */}
                <section className="mb-8">
                    <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-4 flex items-center gap-2">
                        <svg className="w-5 h-5 text-orange-500" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M12.395 2.553a1 1 0 00-1.45-.385c-.345.23-.614.558-.822.88-.214.33-.403.713-.57 1.116-.334.804-.614 1.768-.84 2.734a31.365 31.365 0 00-.613 3.58 2.64 2.64 0 01-.945-1.067c-.328-.68-.398-1.534-.398-2.654A1 1 0 005.05 6.05 6.981 6.981 0 003 11a7 7 0 1011.95-4.95c-.592-.591-.98-.985-1.348-1.467-.363-.476-.724-1.063-1.207-2.03zM12.12 15.12A3 3 0 017 13s.879.5 2.5.5c0-1 .5-4 1.25-4.5.5 1 .786 1.293 1.371 1.879A2.99 2.99 0 0113 13a2.99 2.99 0 01-.879 2.121z" clipRule="evenodd" />
                        </svg>
                        Trending Hashtags
                    </h2>

                    {trendingHashtags.length > 0 ? (
                        <div className="bg-white dark:bg-gray-800 rounded-xl overflow-hidden">
                            {trendingHashtags.map((hashtag, index) => (
                                <Link
                                    key={hashtag.tag}
                                    href={`/search?q=%23${hashtag.tag}`}
                                    className="flex items-center justify-between p-4 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors border-b border-gray-100 dark:border-gray-700 last:border-0"
                                >
                                    <div className="flex items-center gap-3">
                                        <span className="text-gray-400 text-sm font-medium w-6">{index + 1}</span>
                                        <span className="text-blue-500 font-medium">#{hashtag.tag}</span>
                                    </div>
                                    <span className="text-sm text-gray-500 dark:text-gray-400">
                                        {hashtag.count} posts
                                    </span>
                                </Link>
                            ))}
                        </div>
                    ) : (
                        <div className="bg-white dark:bg-gray-800 rounded-xl p-8 text-center text-gray-500 dark:text-gray-400">
                            No trending hashtags yet. Start posting!
                        </div>
                    )}
                </section>

                {/* Suggested Users */}
                <section>
                    <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-4 flex items-center gap-2">
                        <svg className="w-5 h-5 text-blue-500" fill="currentColor" viewBox="0 0 20 20">
                            <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
                        </svg>
                        Suggested for You
                    </h2>

                    {suggestedUsers.length > 0 ? (
                        <div className="bg-white dark:bg-gray-800 rounded-xl overflow-hidden">
                            {suggestedUsers.map((user) => (
                                <Link
                                    key={user.id}
                                    href={`/u/${user.username}`}
                                    className="flex items-center gap-3 p-4 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors border-b border-gray-100 dark:border-gray-700 last:border-0"
                                >
                                    <Avatar src={user.avatarUrl} alt={user.displayName} size="md" />
                                    <div className="flex-1 min-w-0">
                                        <p className="font-semibold text-gray-900 dark:text-white truncate">{user.displayName}</p>
                                        <p className="text-sm text-gray-500 dark:text-gray-400">@{user.username}</p>
                                    </div>
                                    <button className="px-4 py-1.5 bg-blue-500 text-white text-sm font-medium rounded-full hover:bg-blue-600 transition-colors">
                                        Follow
                                    </button>
                                </Link>
                            ))}
                        </div>
                    ) : (
                        <div className="bg-white dark:bg-gray-800 rounded-xl p-8 text-center text-gray-500 dark:text-gray-400">
                            Follow some people to get suggestions!
                        </div>
                    )}
                </section>
            </div>
        </div>
    );
}

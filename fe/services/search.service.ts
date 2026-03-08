import { apiClient } from '@/lib/api';

export interface UserSearchResult {
  id: string;
  username: string;
  displayName: string;
  avatarUrl: string;
  bio: string;
}

export interface PostSearchResult {
  id: string;
  authorId: string;
  authorUsername: string;
  authorDisplayName: string;
  authorAvatarUrl: string;
  content: string;
  hashtags: string[];
  createdAt: string;
}

export interface TrendingHashtag {
  tag: string;
  count: number;
}

export interface SearchResult {
  users: UserSearchResult[];
  posts: PostSearchResult[];
}

/**
 * Search users by query
 */
export const searchUsers = async (query: string, limit = 20): Promise<UserSearchResult[]> => {
  const params = new URLSearchParams({ q: query, limit: String(limit) });
  return apiClient.get<UserSearchResult[]>(`/api/search/users?${params}`);
};

/**
 * Search posts by content or hashtag
 */
export const searchPosts = async (query: string, limit = 20): Promise<PostSearchResult[]> => {
  const params = new URLSearchParams({ q: query, limit: String(limit) });
  return apiClient.get<PostSearchResult[]>(`/api/search/posts?${params}`);
};

/**
 * Search both users and posts
 */
export const searchAll = async (query: string, limit = 10): Promise<SearchResult> => {
  const params = new URLSearchParams({ q: query, limit: String(limit) });
  return apiClient.get<SearchResult>(`/api/search/all?${params}`);
};

/**
 * Get trending hashtags
 */
export const getTrendingHashtags = async (limit = 10): Promise<TrendingHashtag[]> => {
  const params = new URLSearchParams({ limit: String(limit) });
  return apiClient.get<TrendingHashtag[]>(`/api/analytics/trending/hashtags?${params}`);
};

/**
 * Get suggested users to follow
 */
export const getSuggestedUsers = async (limit = 10): Promise<string[]> => {
  const params = new URLSearchParams({ limit: String(limit) });
  const response = await apiClient.get<{ userIds: string[] }>(`/api/social/follows/suggestions?${params}`);
  return response.userIds;
};

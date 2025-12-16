import { apiClient } from '@/lib/api';
import type { AuthResponse, RegisterDto, LoginDto } from '@/types/api';

const AUTH_BASE = '/api/identity';

export const authService = {
    async register(data: RegisterDto): Promise<AuthResponse> {
        return apiClient.post<AuthResponse>(`${AUTH_BASE}/register`, data);
    },

    async login(data: LoginDto): Promise<AuthResponse> {
        return apiClient.post<AuthResponse>(`${AUTH_BASE}/login`, data);
    },

    async refreshToken(refreshToken: string): Promise<AuthResponse> {
        return apiClient.post<AuthResponse>(`${AUTH_BASE}/refresh`, { refreshToken });
    },

    async logout(refreshToken: string): Promise<void> {
        return apiClient.post<void>(`${AUTH_BASE}/revoke`, { refreshToken });
    },

    // Token management in localStorage
    getAccessToken(): string | null {
        if (typeof window === 'undefined') return null;
        return localStorage.getItem('accessToken');
    },

    getRefreshToken(): string | null {
        if (typeof window === 'undefined') return null;
        return localStorage.getItem('refreshToken');
    },

    saveTokens(auth: AuthResponse): void {
        if (typeof window === 'undefined') return;
        localStorage.setItem('accessToken', auth.accessToken);
        localStorage.setItem('refreshToken', auth.refreshToken);
        localStorage.setItem('userId', auth.userId);
    },

    clearTokens(): void {
        if (typeof window === 'undefined') return;
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('userId');
    },

    getUserId(): string | null {
        if (typeof window === 'undefined') return null;
        return localStorage.getItem('userId');
    },
};

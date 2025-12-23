import { apiClient } from '@/lib/api';
import type { AuthResponse, RegisterDto, LoginDto } from '@/types/api';
import Cookies from 'js-cookie';

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

    async verifyEmail(token: string): Promise<void> {
        return apiClient.post<void>(`${AUTH_BASE}/verify-email?token=${token}`, {});
    },

    // Token management with Cookies
    getAccessToken(): string | null {
        return Cookies.get('accessToken') || null;
    },

    getRefreshToken(): string | null {
        return Cookies.get('refreshToken') || null;
    },

    saveTokens(auth: AuthResponse): void {
        const expiresInDays = 7; // Typical refresh token life
        Cookies.set('accessToken', auth.accessToken, { expires: 1, sameSite: 'Strict', secure: process.env.NODE_ENV === 'production' });
        Cookies.set('refreshToken', auth.refreshToken, { expires: expiresInDays, sameSite: 'Strict', secure: process.env.NODE_ENV === 'production' });
        Cookies.set('userId', auth.userId, { expires: expiresInDays, sameSite: 'Strict', secure: process.env.NODE_ENV === 'production' });
    },

    clearTokens(): void {
        Cookies.remove('accessToken');
        Cookies.remove('refreshToken');
        Cookies.remove('userId');
    },

    getUserId(): string | null {
        return Cookies.get('userId') || null;
    },
};

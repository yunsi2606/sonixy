'use client';

import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { authService } from '@/services/auth.service';
import type { AuthResponse, RegisterDto, LoginDto } from '@/types/api';

interface AuthContextType {
    userId: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (data: LoginDto) => Promise<void>;
    register: (data: RegisterDto) => Promise<void>;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [userId, setUserId] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        // Check for existing auth on mount
        const storedUserId = authService.getUserId();
        if (storedUserId) {
            setUserId(storedUserId);
        }
        setIsLoading(false);
    }, []);

    const login = async (data: LoginDto) => {
        const response = await authService.login(data);
        authService.saveTokens(response);
        setUserId(response.userId);
    };

    const register = async (data: RegisterDto) => {
        const response = await authService.register(data);
        authService.saveTokens(response);
        setUserId(response.userId);
    };

    const logout = async () => {
        const refreshToken = authService.getRefreshToken();
        if (refreshToken) {
            try {
                await authService.logout(refreshToken);
            } catch (error) {
                console.error('Logout error:', error);
            }
        }
        authService.clearTokens();
        setUserId(null);
    };

    return (
        <AuthContext.Provider
            value={{
                userId,
                isAuthenticated: !!userId,
                isLoading,
                login,
                register,
                logout,
            }
            }
        >
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within AuthProvider');
    }
    return context;
}

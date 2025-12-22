'use client';

import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { useRouter, usePathname } from 'next/navigation';
import { authService } from '@/services/auth.service';
import { userService } from '@/services/user.service';
import type { AuthResponse, RegisterDto, LoginDto, User } from '@/types/api';

interface AuthContextType {
    user: User | null;
    userId: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (data: LoginDto) => Promise<void>;
    register: (data: RegisterDto) => Promise<void>;
    logout: () => Promise<void>;
    checkAuth: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [userId, setUserId] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const router = useRouter();
    const pathname = usePathname();

    const fetchUser = async () => {
        try {
            const userData = await userService.getCurrentUser();
            setUser(userData);
            return userData;
        } catch (error) {
            console.error('Failed to fetch user:', error);
            // If fetching user fails but we have token, maybe token expired or user deleted?
            // Optionally logout or just leave user null
            return null;
        }
    };

    const checkAuth = async () => {
        const storedUserId = authService.getUserId();
        if (storedUserId) {
            setUserId(storedUserId);
            await fetchUser();
        }
        setIsLoading(false);
    };

    useEffect(() => {
        checkAuth();
    }, []);

    useEffect(() => {
        if (!isLoading && user) {
            // Check if user needs onboarding
            const isNameMissing = !user.firstName || !user.lastName;

            // Avoid redirect loop
            const publicPaths = ['/login', '/register', '/verify-email', '/verify-email-required'];

            if (pathname && !publicPaths.includes(pathname)) {
                if (isNameMissing && pathname !== '/onboarding') {
                    router.push('/onboarding');
                }

                if (user.isEmailVerified === false) {
                    router.push('/verify-email?status=pending&email=' + encodeURIComponent(user.email));
                }
            }
        }
    }, [user, isLoading, pathname, router]);

    const login = async (data: LoginDto) => {
        const response = await authService.login(data);
        authService.saveTokens(response);
        setUserId(response.userId);
        await fetchUser();
    };

    const register = async (data: RegisterDto) => {
        const response = await authService.register(data);
        authService.saveTokens(response);
        setUserId(response.userId);
        await fetchUser();
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
        setUser(null);
        router.push('/login');
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                userId,
                isAuthenticated: !!userId,
                isLoading,
                login,
                register,
                logout,
                checkAuth
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

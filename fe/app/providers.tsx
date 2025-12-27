'use client';

import { AuthProvider } from '@/hooks/useAuth';
import { NotificationProvider } from '@/contexts/NotificationContext';
import { ChatProvider } from '@/contexts/ChatContext';

export function Providers({ children }: { children: React.ReactNode }) {
    return (
        <AuthProvider>
            <NotificationProvider>
                <ChatProvider>
                    {children}
                </ChatProvider>
            </NotificationProvider>
        </AuthProvider>
    );
}

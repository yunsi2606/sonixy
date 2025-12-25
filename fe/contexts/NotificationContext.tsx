'use client';

import React, { createContext, useContext, useEffect, useState, useCallback, useRef } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useAuth } from '@/hooks/useAuth';
import { Notification, NotificationService } from '@/services/notification.service';
import Cookies from 'js-cookie';

interface NotificationContextType {
    notifications: Notification[];
    unreadCount: number;
    isLoading: boolean;
    fetchNotifications: (page?: number) => Promise<void>;
    markAsRead: (id: string) => Promise<void>;
    markAllAsRead: () => Promise<void>;
    connectionState: string;
}

const NotificationContext = createContext<NotificationContextType | undefined>(undefined);

export function NotificationProvider({ children }: { children: React.ReactNode }) {
    const { user } = useAuth();
    const [notifications, setNotifications] = useState<Notification[]>([]);
    const [unreadCount, setUnreadCount] = useState(0);
    const [isLoading, setIsLoading] = useState(false);
    const [connectionState, setConnectionState] = useState('Disconnected');
    const connectionRef = useRef<HubConnection | null>(null);

    // Initial Fetch
    const fetchNotifications = useCallback(async (page: number = 1) => {
        if (!user) return;
        setIsLoading(true);
        try {
            const data = await NotificationService.getNotifications(page);
            if (page === 1) {
                setNotifications(data.items);
            } else {
                setNotifications(prev => [...prev, ...data.items]);
            }
            setUnreadCount(data.unreadCount);
        } catch (error) {
            console.error("Failed to fetch notifications", error);
        } finally {
            setIsLoading(false);
        }
    }, [user]);

    // SignalR Connection
    useEffect(() => {
        if (!user) return;

        const token = Cookies.get('accessToken');
        if (!token) return;

        const connection = new HubConnectionBuilder()
            .withUrl("https://sonixy.nhatcuong.io.vn/hubs/notifications", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        connectionRef.current = connection;

        const startConnection = async () => {
            try {
                await connection.start();
                setConnectionState('Connected');
                console.log("SignalR Connected");

                connection.on("ReceiveNotification", (notification: Notification) => {
                    setNotifications(prev => [notification, ...prev]);
                    setUnreadCount(prev => prev + 1);
                    // Optional: Play sound or show toast
                });

            } catch (err) {
                console.error("SignalR Connection Failed", err);
                setConnectionState('Failed');
                // Retry? AutomaticReconnect handles disconnects, but initial fail needs external retry if desired
            }
        };

        startConnection();

        return () => {
            connection.stop();
        };
    }, [user]);

    // Load initial data on mount/user change
    useEffect(() => {
        if (user) {
            fetchNotifications(1);
        } else {
            setNotifications([]);
            setUnreadCount(0);
        }
    }, [user, fetchNotifications]);

    const markAsRead = async (id: string) => {
        // Optimistic update
        const target = notifications.find(n => n.id === id);
        if (target && !target.isRead) {
            setUnreadCount(prev => Math.max(0, prev - 1));
            setNotifications(prev => prev.map(n => n.id === id ? { ...n, isRead: true } : n));
            await NotificationService.markAsRead(id);
        }
    };

    const markAllAsRead = async () => {
        if (unreadCount === 0) return;

        setUnreadCount(0);
        setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
        await NotificationService.markAllAsRead();
    };

    return (
        <NotificationContext.Provider value={{
            notifications,
            unreadCount,
            isLoading,
            fetchNotifications,
            markAsRead,
            markAllAsRead,
            connectionState
        }}>
            {children}
        </NotificationContext.Provider>
    );
}

export function useNotification() {
    const context = useContext(NotificationContext);
    if (context === undefined) {
        throw new Error('useNotification must be used within a NotificationProvider');
    }
    return context;
}

'use client';

import React from 'react';
import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';
import { useRouter } from 'next/navigation';

export function Sidebar() {
    const { user, logout } = useAuth();
    const router = useRouter();

    const onLogout = async () => {
        if (logout) {
            await logout();
        }
        router.push('/login');
    };

    const profileLink = user?.username ? `/u/${user.username}` : '#';

    return (
        <div className="flex flex-col gap-6">

            {/* Primary Navigation */}
            <nav className="flex flex-col gap-1">
                <NavGroup title="MENU">
                    <NavItem icon="🏠" name="Home" href="/feed" active />
                    <NavItem icon="🌍" name="Explore" href="/explore" />
                    <NavItem icon="🔖" name="Bookmarks" href="/bookmarks" />
                </NavGroup>

                <div className="h-4" />

                <NavGroup title="SOCIAL">
                    <NavItem icon="🔔" name="Notifications" href="/notifications" badge={3} />
                    <NavItem icon="💬" name="Messages" href="/messages" badge={5} />
                    <NavItem icon="👥" name="Communities" href="/communities" />
                </NavGroup>

                <div className="h-4" />

                <NavGroup title="SETTINGS">
                    <NavItem icon="⚙️" name="Settings" href="/settings" />
                    <NavItem icon="❓" name="Help" href="/help" />
                    <button
                        onClick={onLogout}
                        className="w-full flex items-center gap-3 px-4 py-2.5 rounded-lg text-[var(--color-text-secondary)] hover:bg-white/5 hover:text-red-400 transition-all duration-200"
                    >
                        <span className="text-lg opacity-80">🚪</span>
                        <span className="text-sm">Log Out</span>
                    </button>
                </NavGroup>
            </nav>

            {/* Floating Action Button - Desktop */}
            <button className="w-full py-3 mt-4 rounded-xl font-bold text-white shadow-lg bg-gradient-to-r from-[var(--color-primary)] to-[var(--color-secondary)] hover:shadow-[var(--shadow-neon)] transition-all hover:scale-[1.02] active:scale-95">
                Create Post
            </button>

            {/* Mini Profile Footer */}
            <div className="mt-8 pt-6 border-t border-[var(--glass-border)]">
                <Link href={profileLink} className="flex items-center gap-3 p-2 rounded-xl hover:bg-white/5 cursor-pointer transition-colors group">
                    <div className="w-10 h-10 rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] p-[1px]">
                        <img
                            src={user?.avatarUrl || "https://api.dicebear.com/9.x/avataaars/svg?seed=" + (user?.username || "Guest")}
                            alt="Avatar"
                            className="w-full h-full rounded-full bg-[var(--color-bg-deep)] object-cover"
                        />
                    </div>
                    <div className="flex-1 min-w-0">
                        <div className="font-bold text-sm text-white group-hover:text-[var(--color-primary)] transition-colors">
                            {user?.displayName || "My Profile"}
                        </div>
                        <div className="text-xs text-[var(--color-text-muted)] truncate">@{user?.username || "username"}</div>
                    </div>
                </Link>
            </div>
        </div>
    );
}

function NavGroup({ title, children }: { title: string, children: React.ReactNode }) {
    return (
        <div className="flex flex-col gap-1">
            <div className="px-4 text-[10px] font-bold text-[var(--color-text-muted)] tracking-wider mb-2 opacity-70">
                {title}
            </div>
            {children}
        </div>
    );
}

function NavItem({ icon, name, href, active, badge }: any) {
    return (
        <Link
            href={href}
            className={`flex items-center justify-between px-4 py-2.5 rounded-lg transition-all duration-200 ${active
                ? 'bg-white/10 text-white font-medium'
                : 'text-[var(--color-text-secondary)] hover:bg-white/5 hover:text-white'
                }`}
        >
            <div className="flex items-center gap-3">
                <span className="text-lg opacity-80">{icon}</span>
                <span className="text-sm">{name}</span>
            </div>
            {badge && (
                <span className="bg-[var(--color-primary)] text-white text-[10px] font-bold px-1.5 py-0.5 rounded-md">
                    {badge}
                </span>
            )}
        </Link>
    );
}

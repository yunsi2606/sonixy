"use client"

import React from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';
import { Sidebar } from './Sidebar';
import { RightSidebar } from './RightSidebar';

interface GlassShellProps {
    children: React.ReactNode;
}

export const GlassShell: React.FC<GlassShellProps> = ({ children }) => {
    const pathname = usePathname();

    // Logic to determine if sidebars should be shown
    const isSocialPage = pathname.startsWith('/feed') ||
        pathname.startsWith('/u/') ||
        pathname.startsWith('/explore') ||
        pathname.startsWith('/messages') ||
        pathname.startsWith('/notifications') ||
        pathname.startsWith('/bookmarks');

    return (
        <div className="relative h-screen w-full overflow-hidden bg-[var(--color-bg-deep)] text-[var(--color-text-primary)]">
            {/* Dynamic Background */}
            <div className="fixed inset-0 z-0 pointer-events-none">
                <div className="absolute inset-0 bg-gradient-to-br from-[#0B0F1A] via-[#111827] to-[#0B0F1A] opacity-100" />
                <div className="absolute top-[-10%] left-[-10%] w-[50vw] h-[50vw] rounded-full bg-[var(--color-primary)] opacity-[0.03] blur-[120px] animate-[blob-float_25s_infinite_alternate]" />
                <div className="absolute bottom-[-10%] right-[-10%] w-[50vw] h-[50vw] rounded-full bg-[var(--color-secondary)] opacity-[0.04] blur-[120px] animate-[blob-float_30s_infinite_alternate_reverse]" />
                <div className="absolute inset-0 bg-[url('https://grainy-gradients.vercel.app/noise.svg')] opacity-[0.02]" />
            </div>

            {/* Layout Container */}
            {isSocialPage ? (
                // 3-Column Social Layout
                <div className="relative z-10 h-full flex flex-col lg:grid lg:grid-cols-[260px_1fr] xl:grid-cols-[280px_1fr_350px]">

                    {/* Mobile Header (Only visible LG screen down) */}
                    <header className="flex-none lg:hidden glass-base border-b border-[var(--glass-border)] h-16 flex items-center justify-between px-4 z-50">
                        <div className="flex items-center gap-2">
                            <div className="w-8 h-8 rounded-lg bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] flex items-center justify-center text-white font-bold text-lg">S</div>
                            <span className="font-bold text-xl tracking-tight text-white">Sonixy</span>
                        </div>
                        <div className="w-8 h-8 rounded-full bg-white/10" />
                    </header>

                    {/* Left Sidebar (Sticky/Fixed Zone) */}
                    <aside className="hidden lg:flex flex-col h-full border-r border-[var(--glass-border)] bg-black/5 backdrop-blur-sm">
                        <div className="h-20 flex items-center px-6">
                            <div className="flex items-center gap-2">
                                <div className="w-8 h-8 rounded-lg bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] flex items-center justify-center text-white font-bold text-lg shadow-[var(--shadow-neon)]">S</div>
                                <span className="font-bold text-xl tracking-tight bg-gradient-to-r from-white to-white/70 bg-clip-text text-transparent">Sonixy</span>
                            </div>
                        </div>

                        <div className="flex-1 overflow-y-auto custom-scrollbar px-4 pb-4">
                            <Sidebar />
                        </div>
                    </aside>

                    {/* Center Feed (Scrollable Zone) */}
                    <main className="flex-1 h-full overflow-y-auto scroll-smooth">
                        <div className="max-w-[720px] mx-auto w-full px-4 pt-6 pb-24 lg:pb-10">
                            {children}
                        </div>
                    </main>

                    {/* Right Sidebar (Sticky/Fixed Zone) */}
                    <aside className="hidden xl:flex flex-col h-full border-l border-[var(--glass-border)] bg-black/5 backdrop-blur-sm">
                        <div className="flex-1 overflow-y-auto custom-scrollbar p-6">
                            <RightSidebar />
                        </div>
                    </aside>
                </div>
            ) : (
                // Simple Layout for Landing / Auth (Full Width, Scrollable Main)
                <div className="relative z-10 h-full overflow-y-auto scroll-smooth">
                    {children}
                </div>
            )}

            {/* Mobile Bottom Nav (Only on Social Pages and Mobile) */}
            {isSocialPage && (
                <MobileBottomNav />
            )}
        </div>
    );
};


function MobileBottomNav() {
    const { user } = useAuth();

    return (
        <div className="fixed bottom-0 left-0 right-0 h-16 glass-overlay border-t border-[var(--glass-border)] flex lg:hidden items-center justify-around z-50">
            <Link href="/feed" className="flex flex-col items-center gap-1 text-[var(--color-primary)]">
                <span className="text-xl">🏠</span>
            </Link>
            <Link href="/explore" className="flex flex-col items-center gap-1 text-[var(--color-text-secondary)]">
                <span className="text-xl">🌍</span>
            </Link>
            <button className="w-10 h-10 rounded-full bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] flex items-center justify-center shadow-[var(--shadow-neon)] -mt-6 ring-4 ring-[#0B0F1A]">
                <span className="text-white text-xl">+</span>
            </button>
            <Link href="/notifications" className="flex flex-col items-center gap-1 text-[var(--color-text-secondary)]">
                <span className="text-xl">🔔</span>
            </Link>
            <Link href={user ? `/u/${user.username}` : '/login'} className="flex flex-col items-center gap-1 text-[var(--color-text-secondary)]">
                <span className="text-xl">👤</span>
            </Link>
        </div>
    );
}

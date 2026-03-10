import React from 'react';
import { Loader2 } from 'lucide-react';

export default function Loading() {
    return (
        <div className="fixed inset-0 z-[99999] bg-black/40 backdrop-blur-sm flex items-center justify-center animate-in fade-in duration-200">
            <div className="bg-[var(--color-bg-deep)] border border-[var(--glass-border)] rounded-2xl p-8 flex flex-col items-center shadow-2xl animate-in zoom-in-95 duration-300">
                <div className="relative mb-4">
                    <Loader2 size={48} className="text-[var(--color-primary)] animate-spin" />
                    <div className="absolute inset-0 border-t-2 border-[var(--color-secondary)] rounded-full animate-[spin_1.5s_linear_infinite_reverse]" />
                </div>
                <h3 className="text-xl font-bold bg-gradient-to-r from-white to-white/70 bg-clip-text text-transparent">Loading</h3>
                <p className="text-sm text-[var(--color-text-muted)] mt-2">Please wait a moment...</p>
            </div>
        </div>
    );
}

import React from 'react';

export function RightSidebar() {
    return (
        <div className="flex flex-col gap-6 w-full max-w-[320px]">

            {/* Search Input */}
            <div className="relative group">
                <input
                    type="text"
                    placeholder="Search..."
                    className="w-full bg-white/5 border border-[var(--glass-border)] rounded-xl px-10 py-3 text-sm text-white placeholder-white/30 focus:border-[var(--color-primary)] focus:bg-white/10 outline-none transition-all shadow-[var(--shadow-soft)]"
                />
                <div className="absolute left-3 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-[var(--color-primary)] transition-colors">
                    🔍
                </div>
            </div>

            {/* Widget 1: Trending (Short) */}
            <div className="glass-base rounded-2xl p-5 border border-[var(--glass-border)]">
                <div className="flex items-center justify-between mb-4">
                    <h3 className="font-bold text-white text-base">Trending</h3>
                    <button className="text-xs text-[var(--color-primary)] hover:underline">View all</button>
                </div>

                <div className="flex flex-col gap-4">
                    <TrendItem rank="1" tag="#GlassUI" count="12.5k" />
                    <TrendItem rank="2" tag="#WebDesign" count="8.2k" />
                    <TrendItem rank="3" tag="NextJS" count="5.1k" />
                </div>
            </div>

            {/* Widget 2: Suggested People (Tall/Visual) */}
            <div className="glass-base rounded-2xl p-5 border border-[var(--glass-border)]">
                <h3 className="font-bold text-white text-base mb-4">Who to follow</h3>
                <div className="flex flex-col gap-5">
                    <SuggestedUser
                        name="Sarah C."
                        handle="@sarah_design"
                        imgGradient="from-pink-500 to-rose-500"
                    />
                    <SuggestedUser
                        name="Alex Mike"
                        handle="@alex_dev"
                        imgGradient="from-blue-500 to-cyan-500"
                    />
                    <SuggestedUser
                        name="John Doe"
                        handle="@johndoe"
                        imgGradient="from-purple-500 to-indigo-500"
                    />
                </div>
                <button className="w-full mt-4 py-2 rounded-lg bg-white/5 text-xs font-semibold text-[var(--color-text-secondary)] hover:bg-white/10 hover:text-white transition-colors">
                    Show more
                </button>
            </div>

            {/* Footer Links (Asymmetry filler) */}
            <div className="flex flex-wrap gap-x-4 gap-y-2 px-2 text-[11px] text-[var(--color-text-muted)]">
                <a href="#" className="hover:underline">Terms</a>
                <a href="#" className="hover:underline">Privacy</a>
                <a href="#" className="hover:underline">Cookies</a>
                <a href="#" className="hover:underline">Ads Info</a>
                <span>© 2025 Sonixy</span>
            </div>

        </div>
    );
}

function TrendItem({ rank, tag, count }: any) {
    return (
        <div className="flex items-center justify-between group cursor-pointer">
            <div className="flex items-center gap-3">
                <span className="text-xs font-medium text-[var(--color-text-muted)] w-2">{rank}</span>
                <div className="flex flex-col">
                    <span className="font-semibold text-sm text-white group-hover:text-[var(--color-primary)] transition-colors">{tag}</span>
                    <span className="text-[10px] text-[var(--color-text-secondary)]">{count} posts</span>
                </div>
            </div>
            <span className="text-white/20 text-xs">↗</span>
        </div>
    )
}

function SuggestedUser({ name, handle, imgGradient }: any) {
    return (
        <div className="flex items-center gap-3 group">
            <div className={`w-10 h-10 rounded-xl bg-gradient-to-br ${imgGradient} shadow-lg`} />
            <div className="flex-1 min-w-0">
                <div className="font-bold text-sm text-white group-hover:text-[var(--color-secondary)] transition-colors truncate">{name}</div>
                <div className="text-xs text-[var(--color-text-muted)] truncate">{handle}</div>
            </div>
            <button className="p-1.5 rounded-lg bg-white/5 text-white hover:bg-[var(--color-primary)]/20 hover:text-[var(--color-primary)] transition-colors">
                <span className="text-xs font-bold">+</span>
            </button>
        </div>
    )
}

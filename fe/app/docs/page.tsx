'use client';

import React from 'react';
import { useRouter } from 'next/navigation';
import { ArrowLeft, Book, Server, Code, Database, Globe, Container, MessageSquare, Bell, Shield, Users } from 'lucide-react';

export default function DocsPage() {
    const router = useRouter();

    const sections = [
        {
            title: "Microservices Architecture",
            icon: <Server size={24} className="text-[var(--color-primary)]" />,
            items: [
                { name: "Identity Service", desc: "Handles authentication, JWT tokens, and user credentials via ASP.NET Core Identity." },
                { name: "Social Service", desc: "Manages user profiles, relationships (follows), and user search capabilities." },
                { name: "Post Service", desc: "Core logic for creating, liking, and managing posts and multimedia content." },
                { name: "Comment Service", desc: "Handles tiered and nested comment threads for posts." },
                { name: "Search Service", desc: "Elasticsearch-powered global search for users, posts, and hashtags." },
                { name: "Chat Service", desc: "Real-time messaging between users via SignalR." },
                { name: "Notification Service", desc: "Real-time alerts for likes, comments, and messages." },
                { name: "Email Service", desc: "Background processing of verification and promotional emails." },
            ]
        },
        {
            title: "Infrastructure & Tools",
            icon: <Database size={24} className="text-[var(--color-secondary)]" />,
            items: [
                { name: "MongoDB", desc: "Primary NoSQL database for flexible schema and high performance." },
                { name: "RabbitMQ / MassTransit", desc: "Event-driven message broker for asynchronous microservice communication." },
                { name: "Redis", desc: "In-memory caching for news feeds and high-read endpoints." },
                { name: "Elasticsearch", desc: "Full-text and fuzzy search capabilities." },
                { name: "MinIO", desc: "S3-compatible object storage for storing images and videos." },
                { name: "Ocelot", desc: "API Gateway routing external requests to the correct internal microservices." },
            ]
        },
        {
            title: "Frontend Stack",
            icon: <Globe size={24} className="text-pink-500" />,
            items: [
                { name: "Next.js 15", desc: "App Router, Server Actions, and React Server Components for optimal SEO." },
                { name: "Tailwind CSS v4", desc: "Utility-first modern styling with custom glassmorphism plugins." },
                { name: "Zustand", desc: "Lightweight generic state management for complex UI interactions." },
                { name: "Lucide React", desc: "Clean and highly customizable vector icons." }
            ]
        }
    ];

    return (
        <main className="relative z-10 w-full min-h-screen">
            {/* Header / Hero */}
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
                <button
                    onClick={() => router.back()}
                    className="mb-8 flex items-center gap-2 text-[var(--color-text-secondary)] hover:text-white transition-colors glass-base px-4 py-2 rounded-xl w-fit"
                >
                    <ArrowLeft size={20} /> Back to Home
                </button>

                <div className="text-center mb-20 animate-[var(--animate-fade-up)]">
                    <div className="inline-flex items-center justify-center p-4 rounded-3xl bg-gradient-to-br from-white/5 to-white/10 border border-white/10 shadow-[var(--shadow-soft)] mb-6">
                        <Book size={48} className="text-white drop-shadow-md" />
                    </div>
                    <h1 className="text-5xl sm:text-6xl font-black mb-6 leading-tight">
                        <span className="bg-gradient-to-r from-white to-white/70 bg-clip-text text-transparent">System</span>
                        <span className="bg-gradient-to-r from-[var(--color-primary)] to-[var(--color-secondary)] bg-clip-text text-transparent ml-4">Documentation</span>
                    </h1>
                    <p className="text-xl text-[var(--color-text-secondary)] max-w-2xl mx-auto leading-relaxed">
                        Comprehensive overview of the Sonixy architecture, services, and technology stack.
                    </p>
                </div>

                {/* Content Sections */}
                <div className="space-y-16">
                    {sections.map((section, idx) => (
                        <section
                            key={idx}
                            className="glass-float rounded-3xl p-8 sm:p-12 animate-[var(--animate-fade-up)] shadow-[var(--shadow-lift)]"
                            style={{ animationDelay: `${0.1 * (idx + 1)}s` }}
                        >
                            <div className="flex items-center gap-4 mb-10">
                                <div className="p-4 rounded-2xl bg-white/5 border border-white/10 shadow-inner">
                                    {section.icon}
                                </div>
                                <h2 className="text-3xl font-bold text-white relative">
                                    {section.title}
                                    <div className="absolute -bottom-2 left-0 w-1/2 h-1 bg-gradient-to-r from-[var(--color-primary)] to-transparent rounded-full" />
                                </h2>
                            </div>

                            <div className="grid sm:grid-cols-2 lg:grid-cols-2 gap-6">
                                {section.items.map((item, i) => (
                                    <div
                                        key={i}
                                        className="bg-black/20 hover:bg-white/5 border border-transparent hover:border-white/10 rounded-2xl p-6 transition-all duration-300 group"
                                    >
                                        <h3 className="text-xl font-semibold text-white mb-2 flex items-center gap-2">
                                            <div className="w-2 h-2 rounded-full bg-[var(--color-secondary)] group-hover:bg-[var(--color-primary)] transition-colors" />
                                            {item.name}
                                        </h3>
                                        <p className="text-[var(--color-text-secondary)] leading-relaxed pl-4">
                                            {item.desc}
                                        </p>
                                    </div>
                                ))}
                            </div>
                        </section>
                    ))}
                </div>

                {/* API Integration Hint */}
                <section className="mt-20 glass-overlay border border-[var(--color-primary)]/30 rounded-3xl p-10 text-center relative overflow-hidden animate-[var(--animate-scale-in)]">
                    <div className="absolute top-0 right-0 p-32 bg-[var(--color-primary)]/20 blur-[120px] rounded-full pointer-events-none" />
                    <div className="absolute bottom-0 left-0 p-32 bg-[var(--color-secondary)]/10 blur-[120px] rounded-full pointer-events-none" />

                    <div className="relative z-10">
                        <Code size={48} className="text-white mx-auto mb-6 drop-shadow-[0_0_15px_rgba(255,255,255,0.5)]" />
                        <h2 className="text-3xl font-bold text-white mb-4">REST APIs & gRPC</h2>
                        <p className="text-[var(--color-text-secondary)] text-lg max-w-xl mx-auto mb-8">
                            Internal services communicate securely via standard HTTP/2 gRPC channels,
                            while external facing applications interface through Ocelot Gateway routing definitions.
                        </p>
                        <button className="btn-primary px-8 py-3 mx-auto flex items-center gap-2 shadow-[var(--shadow-neon)] hover:scale-105 transition-transform">
                            View API Swagger Specs <ArrowLeft size={16} className="rotate-180" />
                        </button>
                    </div>
                </section>
            </div>
        </main>
    );
}

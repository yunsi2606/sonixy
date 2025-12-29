'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Card } from '@/components/ui/Card';

export default function LoginPage() {
    const router = useRouter();
    const { login } = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            await login({ email, password });
            router.push('/feed');
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Login failed');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <main className="min-h-screen bg-[var(--color-bg-primary)] flex items-center justify-center p-4">
            <div className="w-full max-w-md">
                <div className="text-center mb-8 animate-fade-in">
                    <h1 className="text-4xl font-bold mb-2">
                        <span className="gradient-text">Sonixy</span>
                    </h1>
                    <p className="text-[var(--color-text-secondary)]">Welcome back</p>
                </div>

                <Card className="animate-fade-in" style={{ animationDelay: '0.1s' }} padding="lg">
                    <form onSubmit={handleSubmit} className="space-y-6">
                        <Input
                            id="email"
                            label="Email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                            placeholder="you@example.com"
                        />

                        <Input
                            id="password"
                            label="Password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            placeholder="••••••••"
                        />

                        {error && (
                            <div className="p-3 bg-red-500/10 border border-red-500/30 rounded-xl text-red-400 text-sm">
                                {error}
                            </div>
                        )}

                        <Button
                            type="submit"
                            isLoading={isLoading}
                            fullWidth
                        >
                            Log In
                        </Button>

                        <p className="text-center text-sm text-[var(--color-text-secondary)]">
                            Don't have an account?{' '}
                            <Link
                                href="/register"
                                className="text-[var(--color-primary)] hover:text-[var(--color-primary-hover)] transition-colors"
                            >
                                Sign up
                            </Link>
                        </p>
                    </form>
                </Card>
            </div>
        </main>
    );
}

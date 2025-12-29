'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';
import { userService } from '@/services/user.service';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Card } from '@/components/ui/Card';

export default function RegisterPage() {
    const router = useRouter();
    const { register } = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [username, setUsername] = useState('');

    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    // Username validation
    const [isCheckingUsername, setIsCheckingUsername] = useState(false);
    const [usernameError, setUsernameError] = useState('');
    const [isUsernameAvailable, setIsUsernameAvailable] = useState<boolean | null>(null);

    const checkUsername = async (val: string) => {
        if (!val || val.length < 3) {
            setUsernameError('Username must be at least 3 characters');
            setIsUsernameAvailable(false);
            return;
        }

        setIsCheckingUsername(true);
        setUsernameError('');
        try {
            const available = await userService.checkUsernameAvailability(val);
            if (!available) {
                setUsernameError('Username is already taken');
                setIsUsernameAvailable(false);
            } else {
                setIsUsernameAvailable(true);
            }
        } catch (err) {
            console.error(err);
            // Ignore error for check, but don't mark reliable
        } finally {
            setIsCheckingUsername(false);
        }
    };

    const handleUsernameChange = (val: string) => {
        setUsername(val);
        setIsUsernameAvailable(null);
        setUsernameError('');
        // Simple debounce could be here, but for now rely on onBlur or manual effect if needed.
        // Let's rely on onBlur for API check to save calls, but basic validation here.
        if (val.length > 0 && !/^[a-zA-Z0-9_]+$/.test(val)) {
            setUsernameError('Username can only contain letters, numbers and underscores');
        }
    };

    // Debounce effect for username check
    useEffect(() => {
        const timer = setTimeout(() => {
            if (username.length >= 3 && !usernameError) {
                checkUsername(username);
            }
        }, 500);

        return () => clearTimeout(timer);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [username]);


    const [success, setSuccess] = useState(false);

    // ... (existing username check logic)

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        if (password.length < 8) {
            setError('Password must be at least 8 characters');
            return;
        }

        if (usernameError || !isUsernameAvailable) {
            setError('Please choose a valid available username');
            return;
        }

        setIsLoading(true);

        try {
            await register({ email, username, password });
            setSuccess(true);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Registration failed');
        } finally {
            setIsLoading(false);
        }
    };

    if (success) {
        return (
            <main className="min-h-screen bg-[var(--color-bg-primary)] flex items-center justify-center p-4">
                <div className="w-full max-w-md text-center animate-fade-in">
                    <Card padding="lg">
                        <h1 className="text-3xl font-bold mb-4">Check your email</h1>
                        <p className="text-[var(--color-text-secondary)] mb-6">
                            We've sent a verification link to <strong>{email}</strong>.
                            Please check your inbox and click the link to verify your account.
                        </p>
                        <Link href="/login">
                            <Button fullWidth>
                                Back to Login
                            </Button>
                        </Link>
                    </Card>
                </div>
            </main>
        );
    }

    return (
        <main className="min-h-screen bg-[var(--color-bg-primary)] flex items-center justify-center p-4">
            <div className="w-full max-w-md">
                <div className="text-center mb-8 animate-fade-in">
                    <h1 className="text-4xl font-bold mb-2">
                        <span className="gradient-text">Sonixy</span>
                    </h1>
                    <p className="text-[var(--color-text-secondary)]">Create your account</p>
                </div>

                <Card className="animate-fade-in" style={{ animationDelay: '0.1s' }} padding="lg">
                    <form onSubmit={handleSubmit} className="space-y-6">
                        <div>
                            <Input
                                id="username"
                                label="Username"
                                type="text"
                                value={username}
                                onChange={(e) => handleUsernameChange(e.target.value)}
                                onBlur={() => checkUsername(username)}
                                required
                                minLength={3}
                                maxLength={20}
                                error={usernameError}
                                helperText={
                                    isCheckingUsername ? "Checking availability..." :
                                        isUsernameAvailable && !usernameError ? "Username is available" : undefined
                                }
                                className={isUsernameAvailable === true && !usernameError ? "!border-green-500" : ""}
                                placeholder="username"
                            />
                            {/* Success text isn't handled by Input prop yet directly as a style, but helperText is mapped. 
                                We might need to override the color for success. 
                                For now, relying on boolean classes or just let it be neutral unless error. 
                                Actually, let's keep the manual success message if needed or just trust the green border.
                            */}
                            {isUsernameAvailable && !usernameError && (
                                <p className="text-xs text-green-500 mt-1">Username is available</p>
                            )}
                        </div>

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
                            minLength={8}
                            placeholder="••••••••"
                            helperText="At least 8 characters"
                        />

                        {error && (
                            <div className="p-3 bg-red-500/10 border border-red-500/30 rounded-xl text-red-400 text-sm">
                                {error}
                            </div>
                        )}

                        <Button
                            type="submit"
                            isLoading={isLoading}
                            disabled={!!usernameError || isCheckingUsername}
                            fullWidth
                        >
                            Sign Up
                        </Button>

                        <p className="text-center text-sm text-[var(--color-text-secondary)]">
                            Already have an account?{' '}
                            <Link
                                href="/login"
                                className="text-[var(--color-primary)] hover:text-[var(--color-primary-hover)] transition-colors"
                            >
                                Log in
                            </Link>
                        </p>
                    </form>
                </Card>
            </div>
        </main>
    );
}

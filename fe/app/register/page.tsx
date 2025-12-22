'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';
import { userService } from '@/services/user.service';

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
                    <div className="glass p-8 rounded-2xl">
                        <h1 className="text-3xl font-bold mb-4">Check your email</h1>
                        <p className="text-[var(--color-text-secondary)] mb-6">
                            We've sent a verification link to <strong>{email}</strong>.
                            Please check your inbox and click the link to verify your account.
                        </p>
                        <Link
                            href="/login"
                            className="inline-block px-6 py-3 bg-[var(--color-primary)] hover:bg-[var(--color-primary-hover)] text-white rounded-xl font-medium transition-all"
                        >
                            Back to Login
                        </Link>
                    </div>
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

                <form
                    onSubmit={handleSubmit}
                    // ... (rest of form)

                    className="glass p-8 rounded-2xl space-y-6 animate-fade-in"
                    style={{ animationDelay: '0.1s' }}
                >
                    <div>
                        <label htmlFor="username" className="block text-sm font-medium mb-2">
                            Username
                        </label>
                        <input
                            id="username"
                            type="text"
                            value={username}
                            onChange={(e) => handleUsernameChange(e.target.value)}
                            onBlur={() => checkUsername(username)}
                            required
                            minLength={3}
                            maxLength={20}
                            className={`w-full px-4 py-3 bg-[var(--color-surface)] border rounded-xl focus:outline-none focus:border-[var(--color-border-focus)] transition-colors ${usernameError ? 'border-red-500' : isUsernameAvailable === true ? 'border-green-500' : 'border-[var(--color-border)]'
                                }`}
                            placeholder="username"
                        />
                        {isCheckingUsername && <p className="text-xs text-[var(--color-text-secondary)] mt-1">Checking availability...</p>}
                        {usernameError && <p className="text-xs text-red-500 mt-1">{usernameError}</p>}
                        {isUsernameAvailable && !usernameError && <p className="text-xs text-green-500 mt-1">Username is available</p>}
                    </div>

                    <div>
                        <label htmlFor="email" className="block text-sm font-medium mb-2">
                            Email
                        </label>
                        <input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                            className="w-full px-4 py-3 bg-[var(--color-surface)] border border-[var(--color-border)] rounded-xl focus:outline-none focus:border-[var(--color-border-focus)] transition-colors"
                            placeholder="you@example.com"
                        />
                    </div>

                    <div>
                        <label htmlFor="password" className="block text-sm font-medium mb-2">
                            Password
                        </label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            minLength={8}
                            className="w-full px-4 py-3 bg-[var(--color-surface)] border border-[var(--color-border)] rounded-xl focus:outline-none focus:border-[var(--color-border-focus)] transition-colors"
                            placeholder="••••••••"
                        />
                        <p className="mt-1 text-xs text-[var(--color-text-muted)]">
                            At least 8 characters
                        </p>
                    </div>

                    {error && (
                        <div className="p-3 bg-red-500/10 border border-red-500/30 rounded-xl text-red-400 text-sm">
                            {error}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={isLoading || !!usernameError || isCheckingUsername}
                        className="w-full px-6 py-3 bg-[var(--color-primary)] hover:bg-[var(--color-primary-hover)] text-white rounded-xl font-medium transition-all duration-200 hover:scale-105 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {isLoading ? 'Creating account...' : 'Sign Up'}
                    </button>

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
            </div>
        </main>
    );
}

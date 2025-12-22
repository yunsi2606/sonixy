'use client';

import { Suspense, useEffect, useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';

function VerifyEmailContent() {
    const searchParams = useSearchParams();
    const token = searchParams.get('token');
    const statusParam = searchParams.get('status');
    const emailParam = searchParams.get('email');

    // Determine initial state
    const [status, setStatus] = useState<'verifying' | 'success' | 'error' | 'pending'>(
        statusParam === 'pending' ? 'pending' : (token ? 'verifying' : 'error')
    );
    const [message, setMessage] = useState(
        statusParam === 'pending'
            ? `We sent a verification email to ${emailParam || 'your email address'}. Please check your inbox and click the link to verify your account.`
            : (!token ? 'Invalid verification link.' : '')
    );

    const { logout } = useAuth(); // Use auth hook for logout

    useEffect(() => {
        if (statusParam === 'pending') return; // Skip if just showing pending message

        if (!token) {
            setStatus('error');
            setMessage('Invalid verification link.');
            return;
        }

        const verify = async () => {
            try {
                // Should move URL to env or config
                const res = await fetch(`https://sonixy.nhatcuong.io.vn/api/auth/verify-email?token=${token}`, {
                    method: 'POST'
                });

                if (res.ok) {
                    setStatus('success');
                } else {
                    const data = await res.json();
                    setStatus('error');
                    setMessage(data.error || 'Verification failed.');
                }
            } catch (err) {
                setStatus('error');
                setMessage('Something went wrong. Please try again.');
            }
        };

        verify();
    }, [token, statusParam]);

    return (
        <div className="glass p-8 rounded-2xl w-full max-w-md text-center animate-fade-in">
            {status === 'verifying' && (
                <div>
                    <h1 className="text-2xl font-bold mb-4">Verifying...</h1>
                    <div className="w-8 h-8 border-4 border-[var(--color-primary)] border-t-transparent rounded-full animate-spin mx-auto"></div>
                </div>
            )}

            {status === 'pending' && (
                <div>
                    <h1 className="text-2xl font-bold mb-4 text-[var(--color-primary)]">Verify Your Email</h1>
                    <p className="text-[var(--color-text-secondary)] mb-6">
                        {message}
                    </p>
                    <p className="text-sm text-[var(--color-text-secondary)] mb-6">
                        If you don't see the email, check your spam folder or wait a few minutes.
                    </p>
                    <button
                        onClick={() => logout()}
                        className="text-[var(--color-primary)] hover:underline font-medium"
                    >
                        Log out
                    </button>
                </div>
            )}

            {status === 'success' && (
                <div>
                    <h1 className="text-2xl font-bold mb-4 text-green-500">Email Verified!</h1>
                    <p className="text-[var(--color-text-secondary)] mb-6">
                        Your email has been successfully verified. You can now login.
                    </p>
                    <Link
                        href="/login"
                        className="inline-block px-6 py-3 bg-[var(--color-primary)] hover:bg-[var(--color-primary-hover)] text-white rounded-xl font-medium transition-all"
                    >
                        Go to Login
                    </Link>
                </div>
            )}

            {status === 'error' && (
                <div>
                    <h1 className="text-2xl font-bold mb-4 text-red-500">Verification Failed</h1>
                    <p className="text-[var(--color-text-secondary)] mb-6">
                        {message}
                    </p>
                    <Link
                        href="/login"
                        className="text-[var(--color-primary)] hover:underline block mb-4"
                    >
                        Back to Login
                    </Link>
                    <button
                        onClick={() => logout()}
                        className="text-[var(--color-text-secondary)] hover:text-[var(--color-primary)] text-sm"
                    >
                        Log out
                    </button>
                </div>
            )}
        </div>
    );
}

export default function VerifyEmailPage() {
    return (
        <main className="min-h-screen bg-[var(--color-bg-primary)] flex items-center justify-center p-4">
            <Suspense fallback={<div className="glass p-8 rounded-2xl">Loading...</div>}>
                <VerifyEmailContent />
            </Suspense>
        </main>
    );
}

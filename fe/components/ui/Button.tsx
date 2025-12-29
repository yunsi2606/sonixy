import React from 'react';
import { Loader2 } from 'lucide-react';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
    variant?: 'primary' | 'secondary' | 'ghost' | 'outline';
    size?: 'sm' | 'md' | 'lg';
    isLoading?: boolean;
    fullWidth?: boolean;
}

export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
    ({ className = '', variant = 'primary', size = 'md', isLoading, fullWidth, children, disabled, ...props }, ref) => {

        const baseStyles = "inline-flex items-center justify-center rounded-xl font-medium transition-all duration-200 hover:scale-105 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100 outline-none focus:ring-2 focus:ring-[var(--color-primary)]/50";

        const variants = {
            primary: "bg-[var(--color-primary)] hover:bg-[var(--color-primary-hover)] text-white shadow-lg shadow-[var(--color-primary)]/20",
            secondary: "bg-[var(--color-secondary)] hover:bg-[var(--color-secondary-hover)] text-white",
            ghost: "bg-transparent hover:bg-white/5 text-[var(--color-text-primary)] hover:scale-100",
            outline: "bg-transparent border border-[var(--color-border)] text-[var(--color-text-primary)] hover:bg-white/5"
        };

        const sizes = {
            sm: "px-3 py-1.5 text-sm",
            md: "px-6 py-3 text-base",
            lg: "px-8 py-4 text-lg"
        };

        const widthClass = fullWidth ? 'w-full' : '';

        return (
            <button
                ref={ref}
                className={`${baseStyles} ${variants[variant]} ${sizes[size]} ${widthClass} ${className}`}
                disabled={isLoading || disabled}
                {...props}
            >
                {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                {children}
            </button>
        );
    }
);

Button.displayName = 'Button';

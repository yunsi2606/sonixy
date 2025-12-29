import React, { InputHTMLAttributes } from 'react';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
    label?: string;
    error?: string;
    helperText?: string;
    fullWidth?: boolean;
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
    ({ className = '', label, error, helperText, fullWidth = true, id, ...props }, ref) => {
        return (
            <div className={`${fullWidth ? 'w-full' : ''} ${className}`}>
                {label && (
                    <label htmlFor={id} className="block text-sm font-medium mb-2 text-[var(--color-text-primary)]">
                        {label}
                    </label>
                )}
                <input
                    ref={ref}
                    id={id}
                    className={`
                        w-full px-4 py-3 
                        bg-[var(--color-surface)] 
                        border rounded-xl 
                        focus:outline-none focus:border-[var(--color-border-focus)] 
                        transition-colors
                        placeholder-[var(--color-text-muted)]
                        text-[var(--color-text-primary)]
                        ${error
                            ? 'border-red-500 focus:border-red-500'
                            : 'border-[var(--color-border)]'
                        }
                    `}
                    {...props}
                />
                {error && (
                    <p className="mt-1 text-sm text-red-500">{error}</p>
                )}
                {helperText && !error && (
                    <p className="mt-1 text-xs text-[var(--color-text-muted)]">{helperText}</p>
                )}
            </div>
        );
    }
);

Input.displayName = 'Input';

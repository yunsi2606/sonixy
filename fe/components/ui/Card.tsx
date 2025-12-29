import React from 'react';

interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
    children: React.ReactNode;
    variant?: 'glass' | 'default' | 'flat';
    padding?: 'none' | 'sm' | 'md' | 'lg';
}

export const Card: React.FC<CardProps> = ({
    children,
    className = '',
    variant = 'glass',
    padding = 'md',
    ...props
}) => {

    const variants = {
        glass: "glass border border-[var(--glass-border)]",
        default: "bg-[var(--color-surface)] border border-[var(--color-border)] shadow-sm",
        flat: "bg-transparent border border-[var(--color-border)]"
    };

    const paddings = {
        none: "",
        sm: "p-4",
        md: "p-6",
        lg: "p-8"
    };

    return (
        <div className={`rounded-2xl ${variants[variant]} ${paddings[padding]} ${className}`} {...props}>
            {children}
        </div>
    );
};

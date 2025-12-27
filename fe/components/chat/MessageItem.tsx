import React from 'react';
import { Message, MessageType } from '@/types/chat';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';

interface MessageItemProps {
    message: Message;
    isOwn: boolean;
    showAvatar?: boolean;
    senderName?: string;
    senderAvatar?: string;
}

export const MessageItem: React.FC<MessageItemProps> = ({ message, isOwn, showAvatar, senderName, senderAvatar }) => {
    return (
        <div className={cn("flex w-full mb-4 animate-[var(--animate-scale-in)] origin-bottom", isOwn ? "justify-end" : "justify-start")}>
            {!isOwn && (
                <div className="w-8 h-8 rounded-full bg-gradient-to-br from-[var(--color-primary)] to-[var(--color-secondary)] p-0.5 mr-2 overflow-hidden flex-shrink-0 self-end mb-1">
                    <div className="w-full h-full rounded-full bg-[var(--color-bg-deep)] flex items-center justify-center">
                        {senderAvatar ? <img src={senderAvatar} alt={senderName} className="w-full h-full object-cover" /> : <span className="text-[10px] text-white font-bold">{senderName?.[0]}</span>}
                    </div>
                </div>
            )}
            <div className={cn(
                "max-w-[70%] rounded-2xl px-4 py-2 text-sm shadow-md backdrop-blur-sm",
                isOwn
                    ? "bg-gradient-to-tr from-[var(--color-primary)] to-[var(--color-secondary)] text-white rounded-br-none"
                    : "bg-[var(--color-surface)] border border-[var(--glass-border)] text-[var(--color-text-primary)] rounded-bl-none"
            )}>
                {!isOwn && senderName && <div className="text-[10px] text-[var(--color-text-muted)] mb-1 font-bold">{senderName}</div>}

                {message.type === MessageType.Image ? (
                    <img src={message.content} alt="Attachment" className="max-w-full rounded-lg border border-[var(--glass-border)]" />
                ) : (
                    <p className="whitespace-pre-wrap break-words leading-relaxed">{message.content}</p>
                )}

                <div className={cn("text-[10px] mt-1 text-right", isOwn ? "text-white/70" : "text-[var(--color-text-muted)]")}>
                    {format(new Date(message.createdAt), 'HH:mm')}
                </div>
            </div>
        </div>
    );
};

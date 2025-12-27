import React, { useEffect } from 'react';
import { createPortal } from 'react-dom';
import type { Post } from '@/types/api';
import { CommentSection } from '@/components/comments/CommentSection';
import { X, ChevronLeft, ChevronRight } from 'lucide-react';

interface LightboxProps {
    isOpen: boolean;
    onClose: () => void;
    post: Post;
    initialIndex?: number;
}

export function Lightbox({ isOpen, onClose, post, initialIndex = 0 }: LightboxProps) {
    const [currentIndex, setCurrentIndex] = React.useState(initialIndex);
    const [mounted, setMounted] = React.useState(false);
    const images = post.media || [];

    React.useEffect(() => {
        setMounted(true);
        return () => setMounted(false);
    }, []);

    useEffect(() => {
        if (isOpen) {
            setCurrentIndex(initialIndex);
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = 'unset';
        }
        return () => {
            document.body.style.overflow = 'unset';
        };
    }, [isOpen, initialIndex]);

    if (!mounted || !isOpen) return null;

    const currentImage = images[currentIndex];

    // Navigation handlers
    const next = (e: React.MouseEvent) => {
        e.stopPropagation();
        setCurrentIndex((prev) => (prev + 1) % images.length);
    };

    const prev = (e: React.MouseEvent) => {
        e.stopPropagation();
        setCurrentIndex((prev) => (prev - 1 + images.length) % images.length);
    };

    return createPortal(
        <div
            className="fixed inset-0 z-[9999] bg-black/95 backdrop-blur-md transition-opacity duration-300 flex items-center justify-center p-0 md:p-8"
            onClick={onClose}
        >
            {/* Close Button (Global) */}
            <button
                onClick={onClose}
                className="absolute top-4 right-4 z-[10000] p-2 rounded-full bg-white/10 hover:bg-white/20 text-white transition-colors"
                title="Close"
            >
                <X size={24} />
            </button>

            {/* Main Layout Container */}
            <div
                className="flex flex-col md:flex-row w-full h-full max-w-[1600px] max-h-[95vh] bg-black/50 rounded-xl overflow-hidden shadow-2xl"
                onClick={(e) => e.stopPropagation()}
            >
                {/* LEFT SIDE: Media (Flex Grow) */}
                <div className="relative flex-1 bg-black flex items-center justify-center overflow-hidden min-h-[50vh] md:min-h-0 group">

                    {/* Navigation Buttons (Inside Media Area) */}
                    {images.length > 1 && (
                        <>
                            <button
                                onClick={prev}
                                className="absolute left-4 top-1/2 -translate-y-1/2 z-50 p-3 rounded-full bg-white/10 hover:bg-white/20 text-white transition-all opacity-0 group-hover:opacity-100"
                                title="Previous"
                            >
                                <ChevronLeft size={24} />
                            </button>
                            <button
                                onClick={next}
                                className="absolute right-4 top-1/2 -translate-y-1/2 z-50 p-3 rounded-full bg-white/10 hover:bg-white/20 text-white transition-all opacity-0 group-hover:opacity-100"
                                title="Next"
                            >
                                <ChevronRight size={24} />
                            </button>

                            {/* Image Counter */}
                            <div className="absolute top-4 left-4 z-50 bg-black/50 px-3 py-1 rounded-full text-white text-sm backdrop-blur-sm">
                                {currentIndex + 1} / {images.length}
                            </div>
                        </>
                    )}

                    {/* Content Display */}
                    {currentImage && (
                        currentImage.type === 'video' ? (
                            <video
                                src={currentImage.url}
                                controls
                                autoPlay
                                className="w-full h-full object-contain"
                            />
                        ) : (
                            <img
                                src={currentImage.url}
                                alt="Post content"
                                className="w-full h-full object-contain"
                            />
                        )
                    )}
                </div>

                {/* RIGHT SIDE: Comments (Fixed Width on Desktop) */}
                <div className="hidden md:block w-[380px] lg:w-[420px] flex-shrink-0 bg-[var(--color-bg-deep)] h-full border-l border-[var(--glass-border)] relative z-10">
                    <CommentSection post={post} />
                </div>

                {/* Mobile Comments (Optional: Could be a sliding sheet or below, for now keeping simple stack) */}
                {/* For this strictly split view request, we might hide comments on very small vertically stacked mobile or show them below. 
                     The design requests "Split into 2 fixed sides... Comment side scroll independently". 
                     On mobile vertical stack, the "independent scroll" implies the whole page scrolls or just the comments. 
                     I'll enable comments block on mobile to flow normally below image.
                 */}
                <div className="md:hidden flex-1 bg-[var(--color-bg-deep)] min-h-[50vh]">
                    <CommentSection post={post} />
                </div>
            </div>
        </div>,
        document.body
    );
}

import React, { useEffect } from 'react';
import { createPortal } from 'react-dom';

interface LightboxProps {
    isOpen: boolean;
    onClose: () => void;
    images: { type: 'image' | 'video', url: string }[];
    initialIndex?: number;
}

export function Lightbox({ isOpen, onClose, images, initialIndex = 0 }: LightboxProps) {
    const [currentIndex, setCurrentIndex] = React.useState(initialIndex);
    const [mounted, setMounted] = React.useState(false);

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
            className="fixed inset-0 z-[9999] flex items-center justify-center bg-black/95 backdrop-blur-sm transition-opacity duration-300"
            onClick={onClose}
        >
            {/* Close Button */}
            <button
                onClick={onClose}
                className="absolute top-4 right-4 z-50 p-2 rounded-full bg-white/10 hover:bg-white/20 text-white transition-colors"
                title="Close"
            >
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
            </button>

            {/* Navigation Buttons */}
            {images.length > 1 && (
                <>
                    <button
                        onClick={prev}
                        className="absolute left-4 top-1/2 -translate-y-1/2 z-50 p-3 rounded-full bg-white/10 hover:bg-white/20 text-white transition-colors"
                        title="Previous"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="15 18 9 12 15 6"></polyline></svg>
                    </button>
                    <button
                        onClick={next}
                        className="absolute right-4 top-1/2 -translate-y-1/2 z-50 p-3 rounded-full bg-white/10 hover:bg-white/20 text-white transition-colors"
                        title="Next"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg>
                    </button>
                </>
            )}

            {/* Image Counter */}
            {images.length > 1 && (
                <div className="absolute top-4 left-4 z-50 bg-black/50 px-3 py-1 rounded-full text-white text-sm">
                    {currentIndex + 1} / {images.length}
                </div>
            )}

            {/* Content */}
            <div
                className="relative max-w-[95vw] max-h-[95vh] w-full h-full flex items-center justify-center p-4"
                onClick={(e) => e.stopPropagation()} // Prevent close when clicking content area
            >
                {currentImage.type === 'video' ? (
                    <video
                        src={currentImage.url}
                        controls
                        autoPlay
                        className="max-w-full max-h-full object-contain rounded-lg shadow-2xl"
                    />
                ) : (
                    <img
                        src={currentImage.url}
                        alt="Full screen"
                        className="max-w-full max-h-full object-contain rounded-lg shadow-2xl"
                    />
                )}
            </div>
        </div>,
        document.body
    );
}

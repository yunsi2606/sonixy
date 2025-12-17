import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
    const token = request.cookies.get('accessToken')?.value;
    const { pathname } = request.nextUrl;

    // Paths that require authentication
    const protectedPaths = ['/feed', '/profile', '/messages', '/notifications'];
    const isProtectedPath = protectedPaths.some(path => pathname.startsWith(path));

    // Paths for guests only (redirect to feed if logged in)
    const guestPaths = ['/login', '/register'];
    const isGuestPath = guestPaths.some(path => pathname.startsWith(path));

    if (isProtectedPath && !token) {
        const loginUrl = new URL('/login', request.url);
        // loginUrl.searchParams.set('from', pathname); // Optional: Redirect back after login
        return NextResponse.redirect(loginUrl);
    }

    if (isGuestPath && token) {
        return NextResponse.redirect(new URL('/feed', request.url));
    }

    return NextResponse.next();
}

export const config = {
    matcher: [
        /*
         * Match all request paths except for the ones starting with:
         * - api (API routes)
         * - _next/static (static files)
         * - _next/image (image optimization files)
         * - favicon.ico (favicon file)
         * - public folder
         */
        '/((?!api|_next/static|_next/image|favicon.ico|uploads).*)',
    ],
};

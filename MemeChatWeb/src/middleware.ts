import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
    const token =
        request.cookies.get('auth_token')?.value ||
        request.headers.get('Authorization')?.split(' ')[1];
    const isAuthPage = request.nextUrl.pathname.startsWith('/auth');

    // If trying to access auth pages while logged in
    if (isAuthPage && token) {
        return NextResponse.redirect(new URL('/', request.url));
    }

    // If trying to access protected routes without token
    if (!isAuthPage && !token) {
        return NextResponse.redirect(new URL('/auth/sign-in', request.url));
    }

    return NextResponse.next();
}

export const config = {
    matcher: [
        // Add routes that need protection
        '/',
        '/chat/:path*',
        '/profile/:path*',
        '/settings/:path*',
        // Add auth routes for redirect when logged in
        '/auth/:path*',
    ],
};

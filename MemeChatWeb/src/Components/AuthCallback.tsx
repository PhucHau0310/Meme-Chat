'use client';

import { useEffect } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { useNotification } from '@/hooks/useNotification';
import Cookies from 'js-cookie';

const AuthCallback = () => {
    const router = useRouter();
    const searchParams = useSearchParams();
    const { showNotification } = useNotification();

    useEffect(() => {
        const token = searchParams.get('token');

        if (token) {
            // Store the token in cookies
            Cookies.set('auth_token', token, {
                expires: 7, // Expires in 7 days
                path: '/',
                secure: process.env.NODE_ENV === 'production', // Only use HTTPS in production
                sameSite: 'strict',
            });

            showNotification('Successfully logged in!', 'success', 3);
            router.push('/'); // Redirect to home page
        } else {
            showNotification('Authentication failed', 'error', 3);
            router.push('/auth/sign-in'); // Redirect back to sign in
        }
    }, [router, searchParams, showNotification]);

    return (
        <div className="min-h-screen flex items-center justify-center">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
        </div>
    );
};

export default AuthCallback;

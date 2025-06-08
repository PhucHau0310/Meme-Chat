'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import LeftSideBar from '@/Components/LeftSideBar';
import { getAuthToken } from '@/utils/cookies';

const ChatLayout = ({ children }: { children: React.ReactNode }) => {
    const router = useRouter();

    useEffect(() => {
        const token = getAuthToken();
        if (!token) {
            router.push('/auth/sign-in');
        }
    }, [router]);

    return (
        <div className="w-full h-screen flex flex-col md:flex-row bg-gray-100 dark:bg-gray-900">
            <LeftSideBar />
            <div className="flex-1 p-4">{children}</div>
        </div>
    );
};

export default ChatLayout;

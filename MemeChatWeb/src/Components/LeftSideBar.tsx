'use client';

import DarkModeButton from './DarkModeButton';
import { getAuthToken, removeAuthToken } from '@/utils/cookies';
import LogoIcon from '@/public/logo.svg';
import { Badge } from '@mui/material';
import { menuItems } from '@/constants';
import Link from 'next/link';

const LeftSideBar = () => {
    const handleLogOut = () => {
        const token = getAuthToken();
        fetch('/api/auth/logout', {
            method: 'POST',
            credentials: 'include',
            headers: {
                Authorization: `Bearer ${token}`,
            },
        }).then(() => {
            removeAuthToken();
            window.location.href = '/auth/sign-in';
        });
    };

    return (
        <div className="fixed left-0 top-0 h-screen w-16 md:w-48 bg-white dark:bg-gray-800 border-r flex flex-col justify-between py-4">
            {/* Logo Section */}
            <div className="flex justify-center">
                <img
                    src={LogoIcon.src}
                    alt="Logo"
                    className="w-10 h-10 md:w-12 md:h-12"
                />
            </div>

            {/* Navigation Items */}
            <nav className="flex-1 flex flex-col space-y-2 px-2 mt-8">
                {menuItems.map((item) => {
                    const Icon = item.icon;
                    return (
                        <Link
                            key={item.name}
                            href={item.path}
                            className="flex items-center justify-center mt-8 md:justify-start p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 group relative"
                        >
                            <Badge
                                badgeContent={item.badge}
                                color="primary"
                                invisible={!item.badge}
                            >
                                <Icon className="w-7 h-7" />
                            </Badge>
                            <span className="hidden md:block ml-3 text-sm">
                                {item.name}
                            </span>
                            {/* Tooltip for mobile */}
                            <span className="absolute left-14 bg-gray-900 text-white px-2 py-1 rounded text-xs whitespace-nowrap opacity-0 group-hover:opacity-100 md:hidden">
                                {item.name}
                            </span>
                        </Link>
                    );
                })}
            </nav>

            {/* Bottom Section */}
            <div className="px-2">
                <DarkModeButton />
            </div>
        </div>
    );
};

export default LeftSideBar;

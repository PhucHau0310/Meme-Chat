'use client';

import { useState, useEffect } from 'react';
import { MoonIcon, SunIcon } from '@heroicons/react/24/solid';

export default function DarkModeButton() {
    const [isDark, setIsDark] = useState<boolean>(false);

    useEffect(() => {
        // Kiểm tra trạng thái dark mode từ localStorage hoặc system preference
        const savedMode = localStorage.getItem('theme');
        if (savedMode) {
            setIsDark(savedMode === 'dark');
        } else {
            setIsDark(
                window.matchMedia('(prefers-color-scheme: dark)').matches
            );
        }
    }, []);

    useEffect(() => {
        // Áp dụng data-theme cho html element
        document.documentElement.setAttribute(
            'data-theme',
            isDark ? 'dark' : 'light'
        );
        // Lưu trạng thái vào localStorage
        localStorage.setItem('theme', isDark ? 'dark' : 'light');
    }, [isDark]);

    const toggleDarkMode = () => {
        setIsDark(!isDark);
    };

    return (
        <button
            onClick={toggleDarkMode}
            className="cursor-pointer p-2 rounded-xl bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 transition-colors duration-200"
            aria-label={isDark ? 'Switch to light mode' : 'Switch to dark mode'}
        >
            {isDark ? (
                <SunIcon className="w-6 h-6" />
            ) : (
                <MoonIcon className="w-6 h-6" />
            )}
        </button>
    );
}

'use client';

import React, {
    createContext,
    useCallback,
    useContext,
    ReactNode,
} from 'react';
import { notification } from 'antd';
import type { NotificationType } from '@/types/notification';

interface NotificationContextProps {
    showNotification: (
        message: string,
        type: NotificationType,
        duration?: number
    ) => void;
}

export const NotificationContext = createContext<NotificationContextProps>({
    showNotification: () => {},
});

export const NotificationProvider = ({ children }: { children: ReactNode }) => {
    const [api, contextHolder] = notification.useNotification();

    const showNotification = useCallback(
        (message: string, type: NotificationType, duration: number = 4.5) => {
            api[type]({
                message: type.charAt(0).toUpperCase() + type.slice(1),
                description: message,
                placement: 'topRight',
                duration: duration, // Duration in seconds
            });
        },
        [api]
    );

    return (
        <NotificationContext.Provider value={{ showNotification }}>
            {contextHolder}
            {children}
        </NotificationContext.Provider>
    );
};

export const useNotification = () => {
    const context = useContext(NotificationContext);
    if (!context) {
        throw new Error(
            'useNotification must be used within a NotificationProvider'
        );
    }
    return context;
};

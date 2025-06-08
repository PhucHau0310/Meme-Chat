import { useContext } from 'react';
import { NotificationContext } from '@/providers/NotificationProvider';
import type { NotificationType } from '@/types/notification';

export const useNotification = () => {
    const context = useContext(NotificationContext);

    if (!context) {
        throw new Error(
            'useNotification must be used within NotificationProvider'
        );
    }

    return context;
};

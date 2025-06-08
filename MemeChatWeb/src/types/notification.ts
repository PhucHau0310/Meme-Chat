// src/types/notification.ts

export type NotificationType = 'success' | 'error' | 'warning' | 'info';

export interface NotificationState {
    message: string;
    type: NotificationType;
    isOpen: boolean;
}

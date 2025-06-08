import HomeIcon from '@mui/icons-material/Home';
import PersonIcon from '@mui/icons-material/Person';
import SettingsIcon from '@mui/icons-material/Settings';
import ChatIcon from '@mui/icons-material/Chat';
import LogoutIcon from '@mui/icons-material/Logout';

export interface MenuItem {
    name: string;
    icon: React.ElementType;
    path: string;
    badge?: number;
}

export const menuItems: MenuItem[] = [
    {
        name: 'Home',
        icon: HomeIcon,
        path: '/',
    },
    {
        name: 'Chat',
        icon: ChatIcon,
        path: '/chat',
        badge: 3, // Example notification count
    },
    {
        name: 'Profile',
        icon: PersonIcon,
        path: '/profile',
    },
    {
        name: 'Settings',
        icon: SettingsIcon,
        path: '/settings',
    },
    {
        name: 'Logout',
        icon: LogoutIcon,
        path: '/logout',
    },
];

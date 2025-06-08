import Cookies from 'js-cookie';

export const AUTH_TOKEN_KEY = 'auth_token';

export const getAuthToken = (): string | undefined => {
    return Cookies.get(AUTH_TOKEN_KEY);
};

export const setAuthToken = (token: string): void => {
    Cookies.set(AUTH_TOKEN_KEY, token, {
        expires: 7,
        path: '/',
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'strict',
    });
};

export const removeAuthToken = (): void => {
    Cookies.remove(AUTH_TOKEN_KEY, { path: '/' });
};

'use client';

import Lottie from 'react-lottie-player';
import AnimationLogin from '@/public/AnimationLogin.json';
import GoogleIcon from '@/public/google.svg';
import FacebookIcon from '@/public/facebook.svg';
import GitHubIcon from '@/public/github.svg';
import { useNotification } from '@/hooks/useNotification';

const SignIn = () => {
    const { showNotification } = useNotification();

    const handleSignIn = (e: React.FormEvent) => {
        e.preventDefault();
        showNotification('Feature under development', 'info', 3);
    };

    const handleSocialLogin = async (
        provider: 'google' | 'facebook' | 'github'
    ) => {
        if (!process.env.NEXT_PUBLIC_API_URL) {
            showNotification('API URL not configured', 'error', 3);
            return;
        }

        if (provider === 'facebook' || provider === 'github') {
            showNotification(
                `${
                    provider.charAt(0).toUpperCase() + provider.slice(1)
                } login is not implemented yet`,
                'info',
                3
            );
            return;
        }

        // Construct auth URL with state parameter
        const state = Math.random().toString(36).substring(7);
        sessionStorage.setItem('oauth_state', state);

        const authUrl = new URL(
            `${process.env.NEXT_PUBLIC_API_URL}/api/auth/${provider}-login`
        );
        authUrl.searchParams.append('state', state);
        window.location.href = authUrl.toString();
    };

    return (
        <div className="min-h-screen w-full flex flex-col-reverse md:flex-row justify-center items-center px-4 md:px-6 py-8 md:py-0 gap-8 md:gap-28 max-w-screen-xl mx-auto">
            {/* Sign In Form Section */}
            <div className="w-full md:w-1/2 flex items-center justify-center">
                <div className="w-full max-w-md p-4 md:p-6 rounded-lg shadow-md">
                    <h2 className="text-xl md:text-2xl font-bold mb-6 text-center">
                        Sign In
                    </h2>
                    <form onSubmit={(e) => handleSignIn(e)}>
                        <div className="mb-4">
                            <label
                                className="block text-sm font-medium mb-2"
                                htmlFor="email"
                            >
                                Email
                            </label>
                            <input
                                type="email"
                                id="email"
                                className="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="Enter your email"
                                required
                            />
                        </div>
                        <div className="mb-6">
                            <label
                                className="block text-sm font-medium mb-2"
                                htmlFor="password"
                            >
                                Password
                            </label>
                            <input
                                type="password"
                                id="password"
                                className="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="Enter your password"
                                required
                            />
                        </div>
                        <button
                            type="submit"
                            className="w-full bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition duration-200"
                        >
                            Sign In
                        </button>
                    </form>

                    {/* Social Login Section */}
                    <div className="mt-4 space-y-3">
                        <p className="text-center text-sm text-gray-600">
                            Or sign in with
                        </p>

                        <div className="flex flex-col sm:flex-row gap-2">
                            <button
                                onClick={() => handleSocialLogin('google')}
                                className="flex-1 flex items-center justify-center gap-2 bg-white border border-gray-300 rounded px-4 py-2 text-gray-700 hover:bg-gray-50 transition duration-200"
                            >
                                <img
                                    src={GoogleIcon.src}
                                    alt="Google"
                                    className="w-5 h-5"
                                />
                                <span className="hidden sm:inline">Google</span>
                            </button>
                            <button
                                onClick={() => handleSocialLogin('facebook')}
                                className="flex-1 flex items-center justify-center gap-2 bg-[#1877F2] text-white rounded px-4 py-2 hover:bg-[#1865F2] transition duration-200"
                            >
                                <img
                                    src={FacebookIcon.src}
                                    alt="Facebook"
                                    className="w-5 h-5"
                                />
                                <span className="hidden sm:inline">
                                    Facebook
                                </span>
                            </button>
                            <button
                                onClick={() => handleSocialLogin('github')}
                                className="flex-1 flex items-center justify-center gap-2 bg-[#24292e] text-white rounded px-4 py-2 hover:bg-[#1b1f23] transition duration-200"
                            >
                                <img
                                    src={GitHubIcon.src}
                                    alt="GitHub"
                                    className="w-5 h-5"
                                />
                                <span className="hidden sm:inline">GitHub</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            {/* Animation Section */}
            <div className="w-full md:w-1/2 h-[200px] md:h-screen">
                <Lottie
                    loop
                    animationData={AnimationLogin}
                    play
                    style={{ width: '100%', height: '100%' }}
                />
            </div>
        </div>
    );
};

export default SignIn;

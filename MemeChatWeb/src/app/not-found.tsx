import notfound from '@/public/not-found.jpg';
const NotFoundPage = () => {
    return (
        <main className="flex flex-col items-center justify-center min-h-screen text-center px-4">
            <div className="max-w-lg">
                <h1 className="text-4xl font-extrabold mb-4">
                    404 - Page Not Found
                </h1>
                <p className="text-lg mb-6">
                    Oops! The page you are looking for doesn’t exist or might
                    have been moved.
                </p>
                <img
                    src={notfound.src}
                    alt="Not Found Illustration"
                    className="my-10 w-full max-w-md mx-auto rounded-lg"
                />
            </div>
        </main>
    );
};

export default NotFoundPage;

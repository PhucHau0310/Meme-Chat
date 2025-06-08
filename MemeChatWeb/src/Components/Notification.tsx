const Notification = ({
    status,
    message,
}: {
    status: string;
    message: string;
}) => {
    return (
        <div className={`notification ${status}`}>
            <p>{message}</p>
        </div>
    );
};

export default Notification;

using MemeChatAPI.Models.Entities;

namespace MemeChatAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByGoogleIdAsync(string googleId);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> SetUserOnlineStatusAsync(Guid userId, bool isOnline);
        Task<List<User>> SearchUsersAsync(string searchTerm, Guid currentUserId, int take = 20);
    }
}

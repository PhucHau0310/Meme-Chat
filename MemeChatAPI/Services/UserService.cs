using MemeChatAPI.Data;
using MemeChatAPI.Models.Entities;
using MemeChatAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemeChatAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ChatDbContext _context;

        public UserService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByGoogleIdAsync(string googleId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> SetUserOnlineStatusAsync(Guid userId, bool isOnline)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsOnline = isOnline;
            user.LastSeen = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> SearchUsersAsync(string searchTerm, Guid currentUserId, int take = 20)
        {
            return await _context.Users
                .Where(u => u.Id != currentUserId &&
                           (u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm)))
                .Take(take)
                .ToListAsync();
        }
    }
}

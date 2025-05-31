using MemeChatAPI.Data;
using MemeChatAPI.Models.Entities;
using MemeChatAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemeChatAPI.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly ChatDbContext _context;

        public FriendshipService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<Friendship> SendFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            // Check if friendship already exists
            var existing = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == senderId && f.User2Id == receiverId) ||
                    (f.User1Id == receiverId && f.User2Id == senderId));

            if (existing != null)
                throw new InvalidOperationException("Friendship request already exists");

            var friendship = new Friendship
            {
                User1Id = senderId,
                User2Id = receiverId,
                Status = FriendshipStatus.Pending
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();
            return friendship;
        }

        public async Task<bool> AcceptFriendRequestAsync(Guid friendshipId, Guid userId)
        {
            var friendship = await _context.Friendships.FindAsync(friendshipId);
            if (friendship == null || friendship.User2Id != userId || friendship.Status != FriendshipStatus.Pending)
                return false;

            friendship.Status = FriendshipStatus.Accepted;
            friendship.AcceptedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeclineFriendRequestAsync(Guid friendshipId, Guid userId)
        {
            var friendship = await _context.Friendships.FindAsync(friendshipId);
            if (friendship == null || friendship.User2Id != userId || friendship.Status != FriendshipStatus.Pending)
                return false;

            friendship.Status = FriendshipStatus.Declined;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFriendAsync(Guid userId1, Guid userId2)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == userId1 && f.User2Id == userId2) ||
                    (f.User1Id == userId2 && f.User2Id == userId1));

            if (friendship == null) return false;

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetFriendsAsync(Guid userId)
        {
            var friendships = await _context.Friendships
                .Include(f => f.User1)
                .Include(f => f.User2)
                .Where(f => (f.User1Id == userId || f.User2Id == userId) &&
                           f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            return friendships.Select(f => f.User1Id == userId ? f.User2 : f.User1).ToList();
        }

        public async Task<List<Friendship>> GetPendingRequestsAsync(Guid userId)
        {
            return await _context.Friendships
                .Include(f => f.User1)
                .Include(f => f.User2)
                .Where(f => f.User2Id == userId && f.Status == FriendshipStatus.Pending)
                .ToListAsync();
        }

        public async Task<FriendshipStatus?> GetFriendshipStatusAsync(Guid userId1, Guid userId2)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == userId1 && f.User2Id == userId2) ||
                    (f.User1Id == userId2 && f.User2Id == userId1));

            return friendship?.Status;
        }
    }
}

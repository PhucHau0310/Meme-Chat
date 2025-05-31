using MemeChatAPI.Models.Entities;

namespace MemeChatAPI.Services.Interfaces
{
    public interface IFriendshipService
    {
        Task<Friendship> SendFriendRequestAsync(Guid senderId, Guid receiverId);
        Task<bool> AcceptFriendRequestAsync(Guid friendshipId, Guid userId);
        Task<bool> DeclineFriendRequestAsync(Guid friendshipId, Guid userId);
        Task<bool> RemoveFriendAsync(Guid userId1, Guid userId2);
        Task<List<User>> GetFriendsAsync(Guid userId);
        Task<List<Friendship>> GetPendingRequestsAsync(Guid userId);
        Task<FriendshipStatus?> GetFriendshipStatusAsync(Guid userId1, Guid userId2);
    }
}

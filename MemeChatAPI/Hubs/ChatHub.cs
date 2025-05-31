using MemeChatAPI.Data;
using MemeChatAPI.Models.Entities;
using MemeChatAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MemeChatAPI.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IGroupService _groupService;
        private readonly ChatDbContext _context;

        public ChatHub(IUserService userService, IMessageService messageService,
                       IGroupService groupService, ChatDbContext context)
        {
            _userService = userService;
            _messageService = messageService;
            _groupService = groupService;
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                // Save connection
                var connection = new UserConnection
                {
                    UserId = userId.Value,
                    ConnectionId = Context.ConnectionId,
                    UserAgent = Context.GetHttpContext()?.Request.Headers["User-Agent"],
                    IpAddress = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString()
                };

                _context.UserConnections.Add(connection);
                await _context.SaveChangesAsync();

                // Set user online
                await _userService.SetUserOnlineStatusAsync(userId.Value, true);

                // Join user to their groups
                var groups = await _groupService.GetUserGroupsAsync(userId.Value);
                foreach (var group in groups)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Group_{group.Id}");
                }

                // Notify friends about online status
                await NotifyFriendsAboutOnlineStatus(userId.Value, true);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId != null)
            {
                // Remove connection
                var connection = await _context.UserConnections
                    .FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);
                if (connection != null)
                {
                    _context.UserConnections.Remove(connection);
                    await _context.SaveChangesAsync();
                }

                // Check if user has other connections
                var hasOtherConnections = await _context.UserConnections
                    .AnyAsync(c => c.UserId == userId.Value);

                if (!hasOtherConnections)
                {
                    // Set user offline
                    await _userService.SetUserOnlineStatusAsync(userId.Value, false);
                    await NotifyFriendsAboutOnlineStatus(userId.Value, false);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Send private message
        public async Task SendPrivateMessage(Guid receiverId, string content, string? replyToMessageId = null)
        {
            var senderId = GetUserId();
            if (senderId == null) return;

            var replyToId = !string.IsNullOrEmpty(replyToMessageId) ? (Guid?)Guid.Parse(replyToMessageId) : null;

            var message = await _messageService.SendMessageAsync(
                senderId.Value, content, receiverId, null, MessageType.Text, replyToId);

            var messageDto = new
            {
                Id = message.Id,
                SenderId = message.SenderId,
                Content = message.Content,
                Type = message.Type.ToString(),
                CreatedAt = message.CreatedAt,
                ReplyToMessageId = message.ReplyToMessageId
            };

            // Send to receiver
            var receiverConnections = await GetUserConnectionsAsync(receiverId);
            await Clients.Clients(receiverConnections).SendAsync("ReceivePrivateMessage", messageDto);

            // Send back to sender
            await Clients.Caller.SendAsync("MessageSent", messageDto);
        }

        // Send group message
        public async Task SendGroupMessage(Guid groupId, string content, string? replyToMessageId = null)
        {
            var senderId = GetUserId();
            if (senderId == null) return;

            // Check if user is member of group
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == senderId.Value && gm.IsActive);

            if (!isMember) return;

            var replyToId = !string.IsNullOrEmpty(replyToMessageId) ? (Guid?)Guid.Parse(replyToMessageId) : null;

            var message = await _messageService.SendMessageAsync(
                senderId.Value, content, null, groupId, MessageType.Text, replyToId);

            var messageDto = new
            {
                Id = message.Id,
                SenderId = message.SenderId,
                GroupId = message.GroupId,
                Content = message.Content,
                Type = message.Type.ToString(),
                CreatedAt = message.CreatedAt,
                ReplyToMessageId = message.ReplyToMessageId
            };

            // Send to all group members
            await Clients.Group($"Group_{groupId}").SendAsync("ReceiveGroupMessage", messageDto);
        }

        // Mark message as read
        public async Task MarkMessageAsRead(Guid messageId)
        {
            var userId = GetUserId();
            if (userId == null) return;

            await _messageService.MarkMessageAsReadAsync(messageId, userId.Value);

            // Notify sender about read status
            var message = await _messageService.GetMessageByIdAsync(messageId);
            if (message != null)
            {
                var senderConnections = await GetUserConnectionsAsync(message.SenderId);
                await Clients.Clients(senderConnections).SendAsync("MessageRead", new
                {
                    MessageId = messageId,
                    ReadByUserId = userId.Value,
                    ReadAt = DateTime.UtcNow
                });
            }
        }

        // Add emoji reaction
        public async Task AddReaction(Guid messageId, string emoji)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var reaction = await _messageService.AddReactionAsync(messageId, userId.Value, emoji);

            var reactionDto = new
            {
                MessageId = messageId,
                UserId = userId.Value,
                Emoji = emoji,
                CreatedAt = reaction.CreatedAt
            };

            // Get message to determine if it's private or group
            var message = await _messageService.GetMessageByIdAsync(messageId);
            if (message != null)
            {
                if (message.GroupId.HasValue)
                {
                    await Clients.Group($"Group_{message.GroupId.Value}").SendAsync("ReactionAdded", reactionDto);
                }
                else if (message.ReceiverId.HasValue)
                {
                    var receiverConnections = await GetUserConnectionsAsync(message.ReceiverId.Value);
                    var senderConnections = await GetUserConnectionsAsync(message.SenderId);

                    var allConnections = receiverConnections.Concat(senderConnections).Distinct().ToList();
                    await Clients.Clients(allConnections).SendAsync("ReactionAdded", reactionDto);
                }
            }
        }

        // Remove emoji reaction
        public async Task RemoveReaction(Guid messageId, string emoji)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var success = await _messageService.RemoveReactionAsync(messageId, userId.Value, emoji);

            if (success)
            {
                var reactionDto = new
                {
                    MessageId = messageId,
                    UserId = userId.Value,
                    Emoji = emoji
                };

                // Get message to determine if it's private or group
                var message = await _messageService.GetMessageByIdAsync(messageId);
                if (message != null)
                {
                    if (message.GroupId.HasValue)
                    {
                        await Clients.Group($"Group_{message.GroupId.Value}").SendAsync("ReactionRemoved", reactionDto);
                    }
                    else if (message.ReceiverId.HasValue)
                    {
                        var receiverConnections = await GetUserConnectionsAsync(message.ReceiverId.Value);
                        var senderConnections = await GetUserConnectionsAsync(message.SenderId);

                        var allConnections = receiverConnections.Concat(senderConnections).Distinct().ToList();
                        await Clients.Clients(allConnections).SendAsync("ReactionRemoved", reactionDto);
                    }
                }
            }
        }

        // User is typing
        public async Task StartTyping(Guid? receiverId = null, Guid? groupId = null)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var typingDto = new { UserId = userId.Value, IsTyping = true };

            if (groupId.HasValue)
            {
                await Clients.Group($"Group_{groupId.Value}").SendAsync("UserTyping", typingDto);
            }
            else if (receiverId.HasValue)
            {
                var receiverConnections = await GetUserConnectionsAsync(receiverId.Value);
                await Clients.Clients(receiverConnections).SendAsync("UserTyping", typingDto);
            }
        }

        public async Task StopTyping(Guid? receiverId = null, Guid? groupId = null)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var typingDto = new { UserId = userId.Value, IsTyping = false };

            if (groupId.HasValue)
            {
                await Clients.Group($"Group_{groupId.Value}").SendAsync("UserTyping", typingDto);
            }
            else if (receiverId.HasValue)
            {
                var receiverConnections = await GetUserConnectionsAsync(receiverId.Value);
                await Clients.Clients(receiverConnections).SendAsync("UserTyping", typingDto);
            }
        }

        // Join group (when user is added to group)
        public async Task JoinGroup(Guid groupId)
        {
            var userId = GetUserId();
            if (userId == null) return;

            // Check if user is member
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId.Value && gm.IsActive);

            if (isMember)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Group_{groupId}");
            }
        }

        // Leave group
        public async Task LeaveGroup(Guid groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Group_{groupId}");
        }

        // Private helper methods
        private Guid? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        }

        private async Task<List<string>> GetUserConnectionsAsync(Guid userId)
        {
            return await _context.UserConnections
                .Where(c => c.UserId == userId)
                .Select(c => c.ConnectionId)
                .ToListAsync();
        }

        private async Task NotifyFriendsAboutOnlineStatus(Guid userId, bool isOnline)
        {
            var friendships = await _context.Friendships
                .Where(f => (f.User1Id == userId || f.User2Id == userId) &&
                           f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            var friendIds = friendships.Select(f => f.User1Id == userId ? f.User2Id : f.User1Id).ToList();

            foreach (var friendId in friendIds)
            {
                var friendConnections = await GetUserConnectionsAsync(friendId);
                await Clients.Clients(friendConnections).SendAsync("FriendOnlineStatusChanged", new
                {
                    UserId = userId,
                    IsOnline = isOnline,
                    LastSeen = DateTime.UtcNow
                });
            }
        }
    }
}

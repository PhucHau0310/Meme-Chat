using MemeChatAPI.Data;
using MemeChatAPI.Models.Entities;
using MemeChatAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemeChatAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly ChatDbContext _context;

        public MessageService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<Message> SendMessageAsync(Guid senderId, string? content, Guid? receiverId = null,
            Guid? groupId = null, MessageType type = MessageType.Text, Guid? replyToMessageId = null)
        {
            var message = new Message
            {
                SenderId = senderId,
                Content = content,
                ReceiverId = receiverId,
                GroupId = groupId,
                Type = type,
                ReplyToMessageId = replyToMessageId
            };

            _context.Messages.Add(message);

            // Create message status for sender
            var senderStatus = new MessageStatus
            {
                MessageId = message.Id,
                UserId = senderId,
                Status = MessageStatusType.Sent
            };
            _context.MessageStatuses.Add(senderStatus);

            // Create message status for receiver(s)
            if (receiverId.HasValue)
            {
                var receiverStatus = new MessageStatus
                {
                    MessageId = message.Id,
                    UserId = receiverId.Value,
                    Status = MessageStatusType.Delivered
                };
                _context.MessageStatuses.Add(receiverStatus);
            }
            else if (groupId.HasValue)
            {
                var groupMembers = await _context.GroupMembers
                    .Where(gm => gm.GroupId == groupId.Value && gm.UserId != senderId && gm.IsActive)
                    .ToListAsync();

                foreach (var member in groupMembers)
                {
                    var memberStatus = new MessageStatus
                    {
                        MessageId = message.Id,
                        UserId = member.UserId,
                        Status = MessageStatusType.Delivered
                    };
                    _context.MessageStatuses.Add(memberStatus);
                }
            }

            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetMessagesAsync(Guid? receiverId = null, Guid? groupId = null, int skip = 0, int take = 50)
        {
            var query = _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .Include(m => m.Reactions)
                .Include(m => m.MessageStatuses)
                .Where(m => !m.IsDeleted);

            if (receiverId.HasValue)
                query = query.Where(m => m.ReceiverId == receiverId.Value);
            else if (groupId.HasValue)
                query = query.Where(m => m.GroupId == groupId.Value);

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId)
        {
            var status = await _context.MessageStatuses
                .FirstOrDefaultAsync(ms => ms.MessageId == messageId && ms.UserId == userId);

            if (status == null) return false;

            status.Status = MessageStatusType.Read;
            status.StatusTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkMessagesAsDeliveredAsync(Guid userId, List<Guid> messageIds)
        {
            var statuses = await _context.MessageStatuses
                .Where(ms => messageIds.Contains(ms.MessageId) && ms.UserId == userId)
                .ToListAsync();

            foreach (var status in statuses)
            {
                if (status.Status == MessageStatusType.Sent)
                {
                    status.Status = MessageStatusType.Delivered;
                    status.StatusTime = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Message?> GetMessageByIdAsync(Guid messageId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .Include(m => m.Reactions)
                .Include(m => m.MessageStatuses)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId, Guid userId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null || message.SenderId != userId) return false;

            message.IsDeleted = true;
            message.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MessageReaction> AddReactionAsync(Guid messageId, Guid userId, string emoji)
        {
            // Remove existing reaction with same emoji from same user
            var existing = await _context.MessageReactions
                .FirstOrDefaultAsync(mr => mr.MessageId == messageId && mr.UserId == userId && mr.Emoji == emoji);

            if (existing != null)
                return existing;

            var reaction = new MessageReaction
            {
                MessageId = messageId,
                UserId = userId,
                Emoji = emoji
            };

            _context.MessageReactions.Add(reaction);
            await _context.SaveChangesAsync();
            return reaction;
        }

        public async Task<bool> RemoveReactionAsync(Guid messageId, Guid userId, string emoji)
        {
            var reaction = await _context.MessageReactions
                .FirstOrDefaultAsync(mr => mr.MessageId == messageId && mr.UserId == userId && mr.Emoji == emoji);

            if (reaction == null) return false;

            _context.MessageReactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

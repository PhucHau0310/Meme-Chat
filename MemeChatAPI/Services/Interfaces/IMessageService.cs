using MemeChatAPI.Models.Entities;

namespace MemeChatAPI.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Message> SendMessageAsync(Guid senderId, string? content, Guid? receiverId = null, Guid? groupId = null,
            MessageType type = MessageType.Text, Guid? replyToMessageId = null);
        Task<List<Message>> GetMessagesAsync(Guid? receiverId = null, Guid? groupId = null, int skip = 0, int take = 50);
        Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId);
        Task<bool> MarkMessagesAsDeliveredAsync(Guid userId, List<Guid> messageIds);
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task<bool> DeleteMessageAsync(Guid messageId, Guid userId);
        Task<MessageReaction> AddReactionAsync(Guid messageId, Guid userId, string emoji);
        Task<bool> RemoveReactionAsync(Guid messageId, Guid userId, string emoji);
    }
}

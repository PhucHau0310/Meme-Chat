using MemeChatAPI.Models.Entities;

namespace MemeChatAPI.Services.Interfaces
{
    public interface IFileService
    {
        Task<MessageAttachment> SaveFileAsync(Guid messageId, IFormFile file);
        Task<string> GetFilePathAsync(Guid attachmentId);
        Task<bool> DeleteFileAsync(Guid attachmentId);
    }
}

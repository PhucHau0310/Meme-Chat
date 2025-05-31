using MemeChatAPI.Data;
using MemeChatAPI.Models.Entities;
using MemeChatAPI.Services.Interfaces;

namespace MemeChatAPI.Services
{
    public class FileService : IFileService
    {
        private readonly ChatDbContext _context;
        private readonly string _uploadPath;

        public FileService(ChatDbContext context, IConfiguration configuration)
        {
            _context = context;
            _uploadPath = configuration["FileUpload:Path"] ?? "wwwroot/uploads";
        }

        public async Task<MessageAttachment> SaveFileAsync(Guid messageId, IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_uploadPath, fileName);

            Directory.CreateDirectory(_uploadPath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new MessageAttachment
            {
                MessageId = messageId,
                FileName = file.FileName,
                FilePath = filePath,
                ContentType = file.ContentType,
                FileSize = file.Length
            };

            // Set dimensions for images
            if (file.ContentType.StartsWith("image/"))
            {
                // Add image processing logic here if needed
            }

            _context.MessageAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }

        public async Task<string> GetFilePathAsync(Guid attachmentId)
        {
            var attachment = await _context.MessageAttachments.FindAsync(attachmentId);
            return attachment?.FilePath ?? string.Empty;
        }

        public async Task<bool> DeleteFileAsync(Guid attachmentId)
        {
            var attachment = await _context.MessageAttachments.FindAsync(attachmentId);
            if (attachment == null) return false;

            if (File.Exists(attachment.FilePath))
            {
                File.Delete(attachment.FilePath);
            }

            _context.MessageAttachments.Remove(attachment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

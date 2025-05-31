using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MemeChatAPI.Models.Entities
{
    public class MessageAttachment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid MessageId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public int? Width { get; set; } // Cho hình ảnh
        public int? Height { get; set; } // Cho hình ảnh
        public int? Duration { get; set; } // Cho audio/video (giây)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        // Navigation properties
        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; } = null!;
    }
}

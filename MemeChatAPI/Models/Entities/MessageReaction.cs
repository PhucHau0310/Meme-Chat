using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MemeChatAPI.Models.Entities
{
    public class MessageReaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid MessageId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Emoji { get; set; } = string.Empty; // Unicode emoji

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        // Navigation properties
        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}

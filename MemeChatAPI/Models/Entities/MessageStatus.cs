using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MemeChatAPI.Models.Entities
{
    public enum MessageStatusType
    {
        Sent = 0,
        Delivered = 1,
        Read = 2
    }
    public class MessageStatus
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid MessageId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public MessageStatusType Status { get; set; } = MessageStatusType.Sent;
        public DateTime StatusTime { get; set; } = DateTime.UtcNow.AddHours(7);

        // Navigation properties
        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MemeChatAPI.Models.Entities
{
    public enum MessageType
    {
        Text = 0,
        Image = 1,
        File = 2,
        Audio = 3,
        Video = 4,
        System = 5 // Tin nhắn hệ thống (vào nhóm, rời nhóm, etc.)
    }

    public class Message
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SenderId { get; set; }

        public Guid? GroupId { get; set; } // Null nếu là tin nhắn riêng tư
        public Guid? ReceiverId { get; set; } // Null nếu là tin nhắn nhóm

        [MaxLength(2000)]
        public string? Content { get; set; } // Text content

        public MessageType Type { get; set; } = MessageType.Text;

        public Guid? ReplyToMessageId { get; set; } // Trả lời tin nhắn

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public virtual User? Receiver { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }

        [ForeignKey("ReplyToMessageId")]
        public virtual Message? ReplyToMessage { get; set; }

        public virtual ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
        public virtual ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();
        public virtual ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MemeChatAPI.Models.Entities
{
    public enum ConversationType
    {
        Private = 0, // Chat riêng tư 1-1
        Group = 1    // Chat nhóm
    }

    public class Conversation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? User1Id { get; set; } // Cho chat riêng tư
        public Guid? User2Id { get; set; } // Cho chat riêng tư
        public Guid? GroupId { get; set; } // Cho chat nhóm

        public ConversationType Type { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; } // Tên cuộc trò chuyện

        public Guid? LastMessageId { get; set; }
        public DateTime LastActivity { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        // Navigation properties
        [ForeignKey("User1Id")]
        public virtual User? User1 { get; set; }

        [ForeignKey("User2Id")]
        public virtual User? User2 { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }

        [ForeignKey("LastMessageId")]
        public virtual Message? LastMessage { get; set; }

        public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    }
}

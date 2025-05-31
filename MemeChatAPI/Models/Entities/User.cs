using System.ComponentModel.DataAnnotations;

namespace MemeChatAPI.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Avatar { get; set; }

        [MaxLength(255)]
        public string? GoogleId { get; set; } // Cho Google OAuth

        public bool IsOnline { get; set; } = false;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        // Navigation properties
        public virtual ICollection<Friendship> FriendsAsUser1 { get; set; } = new List<Friendship>();
        public virtual ICollection<Friendship> FriendsAsUser2 { get; set; } = new List<Friendship>();
        public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public virtual ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();
    }
}

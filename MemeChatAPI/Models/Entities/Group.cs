using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MemeChatAPI.Models.Entities
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? Avatar { get; set; }

        [Required]
        public Guid CreatedByUserId { get; set; }

        public bool IsPrivate { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        // Navigation properties
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}

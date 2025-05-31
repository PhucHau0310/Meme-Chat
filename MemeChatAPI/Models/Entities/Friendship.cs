using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MemeChatAPI.Models.Entities
{
    public enum FriendshipStatus
    {
        Pending = 0,
        Accepted = 1,
        Blocked = 2,
        Declined = 3
    }

    public class Friendship
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid User1Id { get; set; }

        [Required]
        public Guid User2Id { get; set; }

        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime? AcceptedAt { get; set; }

        // Navigation properties
        [ForeignKey("User1Id")]
        public virtual User User1 { get; set; } = null!;

        [ForeignKey("User2Id")]
        public virtual User User2 { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MemeChatAPI.Models.Entities
{
    public class UserConnection
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ConnectionId { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? UserAgent { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime LastActivity { get; set; } = DateTime.UtcNow.AddHours(7);

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}

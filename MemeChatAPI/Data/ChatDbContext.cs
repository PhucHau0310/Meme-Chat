using MemeChatAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemeChatAPI.Data
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageAttachment> MessageAttachments { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.GoogleId).IsUnique();
            });

            // Friendship configurations
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.HasOne(f => f.User1)
                    .WithMany(u => u.FriendsAsUser1)
                    .HasForeignKey(f => f.User1Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.User2)
                    .WithMany(u => u.FriendsAsUser2)
                    .HasForeignKey(f => f.User2Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.User1Id, e.User2Id }).IsUnique();
            });

            // Group configurations
            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasOne(g => g.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(g => g.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // GroupMember configurations
            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.HasOne(gm => gm.Group)
                    .WithMany(g => g.Members)
                    .HasForeignKey(gm => gm.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(gm => gm.User)
                    .WithMany(u => u.GroupMemberships)
                    .HasForeignKey(gm => gm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.GroupId, e.UserId }).IsUnique();
            });

            // Message configurations
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Receiver)
                    .WithMany()
                    .HasForeignKey(m => m.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Group)
                    .WithMany(g => g.Messages)
                    .HasForeignKey(m => m.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.ReplyToMessage)
                    .WithMany()
                    .HasForeignKey(m => m.ReplyToMessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.SenderId, e.ReceiverId });
                entity.HasIndex(e => e.GroupId);
            });

            // MessageAttachment configurations
            modelBuilder.Entity<MessageAttachment>(entity =>
            {
                entity.HasOne(ma => ma.Message)
                    .WithMany(m => m.Attachments)
                    .HasForeignKey(ma => ma.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MessageStatus configurations
            modelBuilder.Entity<MessageStatus>(entity =>
            {
                entity.HasOne(ms => ms.Message)
                    .WithMany(m => m.MessageStatuses)
                    .HasForeignKey(ms => ms.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ms => ms.User)
                    .WithMany(u => u.MessageStatuses)
                    .HasForeignKey(ms => ms.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.MessageId, e.UserId }).IsUnique();
            });

            // MessageReaction configurations
            modelBuilder.Entity<MessageReaction>(entity =>
            {
                entity.HasOne(mr => mr.Message)
                    .WithMany(m => m.Reactions)
                    .HasForeignKey(mr => mr.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mr => mr.User)
                    .WithMany()
                    .HasForeignKey(mr => mr.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.MessageId, e.UserId, e.Emoji }).IsUnique();
            });

            // Conversation configurations
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasOne(c => c.User1)
                    .WithMany()
                    .HasForeignKey(c => c.User1Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User2)
                    .WithMany()
                    .HasForeignKey(c => c.User2Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Group)
                    .WithMany()
                    .HasForeignKey(c => c.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.LastMessage)
                    .WithMany()
                    .HasForeignKey(c => c.LastMessageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ConversationParticipant configurations
            modelBuilder.Entity<ConversationParticipant>(entity =>
            {
                entity.HasOne(cp => cp.Conversation)
                    .WithMany(c => c.Participants)
                    .HasForeignKey(cp => cp.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.User)
                    .WithMany()
                    .HasForeignKey(cp => cp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ConversationId, e.UserId }).IsUnique();
            });

            // UserConnection configurations
            modelBuilder.Entity<UserConnection>(entity =>
            {
                entity.HasOne(uc => uc.User)
                    .WithMany()
                    .HasForeignKey(uc => uc.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ConnectionId).IsUnique();
                entity.HasIndex(e => e.UserId);
            });

            // Notification configurations
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.IsRead });
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}

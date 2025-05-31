using MemeChatAPI.Data;
using MemeChatAPI.Models.Entities;
using MemeChatAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemeChatAPI.Services
{
    public class GroupService : IGroupService
    {
        private readonly ChatDbContext _context;

        public GroupService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<Group> CreateGroupAsync(string name, string? description, Guid createdByUserId, bool isPrivate = false)
        {
            var group = new Group
            {
                Name = name,
                Description = description,
                CreatedByUserId = createdByUserId,
                IsPrivate = isPrivate
            };

            _context.Groups.Add(group);

            // Add creator as owner
            var ownerMember = new GroupMember
            {
                GroupId = group.Id,
                UserId = createdByUserId,
                Role = GroupMemberRole.Owner
            };
            _context.GroupMembers.Add(ownerMember);

            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<GroupMember> AddMemberAsync(Guid groupId, Guid userId, GroupMemberRole role = GroupMemberRole.Member)
        {
            // Check if user is already a member
            var existing = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (existing != null)
            {
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    existing.JoinedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                return existing;
            }

            var member = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                Role = role
            };

            _context.GroupMembers.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<bool> RemoveMemberAsync(Guid groupId, Guid userId, Guid removedByUserId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (member == null) return false;

            // Check permissions
            var remover = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == removedByUserId);

            if (remover == null || (remover.Role != GroupMemberRole.Admin && remover.Role != GroupMemberRole.Owner))
                return false;

            // Can't remove owner
            if (member.Role == GroupMemberRole.Owner) return false;

            member.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMemberRoleAsync(Guid groupId, Guid userId, GroupMemberRole newRole, Guid updatedByUserId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (member == null) return false;

            // Check permissions
            var updater = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == updatedByUserId);

            if (updater == null || updater.Role != GroupMemberRole.Owner) return false;

            member.Role = newRole;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Group>> GetUserGroupsAsync(Guid userId)
        {
            return await _context.GroupMembers
                .Include(gm => gm.Group)
                .Where(gm => gm.UserId == userId && gm.IsActive)
                .Select(gm => gm.Group)
                .ToListAsync();
        }

        public async Task<List<GroupMember>> GetGroupMembersAsync(Guid groupId)
        {
            return await _context.GroupMembers
                .Include(gm => gm.User)
                .Where(gm => gm.GroupId == groupId && gm.IsActive)
                .ToListAsync();
        }

        public async Task<Group?> GetGroupByIdAsync(Guid groupId)
        {
            return await _context.Groups
                .Include(g => g.Members.Where(m => m.IsActive))
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<bool> LeaveGroupAsync(Guid groupId, Guid userId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (member == null) return false;

            // If owner is leaving, transfer ownership to an admin or remove group
            if (member.Role == GroupMemberRole.Owner)
            {
                var newOwner = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.Role == GroupMemberRole.Admin && gm.IsActive);

                if (newOwner != null)
                {
                    newOwner.Role = GroupMemberRole.Owner;
                }
            }

            member.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

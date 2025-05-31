using MemeChatAPI.Models.Entities;

namespace MemeChatAPI.Services.Interfaces
{
    public interface IGroupService
    {
        Task<Group> CreateGroupAsync(string name, string? description, Guid createdByUserId, bool isPrivate = false);
        Task<GroupMember> AddMemberAsync(Guid groupId, Guid userId, GroupMemberRole role = GroupMemberRole.Member);
        Task<bool> RemoveMemberAsync(Guid groupId, Guid userId, Guid removedByUserId);
        Task<bool> UpdateMemberRoleAsync(Guid groupId, Guid userId, GroupMemberRole newRole, Guid updatedByUserId);
        Task<List<Group>> GetUserGroupsAsync(Guid userId);
        Task<List<GroupMember>> GetGroupMembersAsync(Guid groupId);
        Task<Group?> GetGroupByIdAsync(Guid groupId);
        Task<bool> LeaveGroupAsync(Guid groupId, Guid userId);
    }
}

using MemeChatAPI.Models.DTO;
using MemeChatAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MemeChatAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var group = await _groupService.CreateGroupAsync(
                request.Name, request.Description, userId.Value, request.IsPrivate);

            return Ok(group);
        }

        [HttpPost("{groupId}/members")]
        public async Task<IActionResult> AddMember(Guid groupId, [FromBody] AddMemberRequest request)
        {
            var member = await _groupService.AddMemberAsync(groupId, request.UserId);
            return Ok(member);
        }

        [HttpDelete("{groupId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var success = await _groupService.RemoveMemberAsync(groupId, userId, currentUserId.Value);
            return success ? Ok() : BadRequest("Cannot remove member");
        }

        [HttpGet]
        public async Task<IActionResult> GetUserGroups()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var groups = await _groupService.GetUserGroupsAsync(userId.Value);
            return Ok(groups);
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroup(Guid groupId)
        {
            var group = await _groupService.GetGroupByIdAsync(groupId);
            if (group == null) return NotFound();

            return Ok(group);
        }

        [HttpGet("{groupId}/members")]
        public async Task<IActionResult> GetGroupMembers(Guid groupId)
        {
            var members = await _groupService.GetGroupMembersAsync(groupId);
            return Ok(members);
        }

        [HttpPost("{groupId}/leave")]
        public async Task<IActionResult> LeaveGroup(Guid groupId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _groupService.LeaveGroupAsync(groupId, userId.Value);
            return success ? Ok() : BadRequest("Cannot leave group");
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        }
    }
}

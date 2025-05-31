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
    public class FriendsController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;

        public FriendsController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestModel request)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var friendship = await _friendshipService.SendFriendRequestAsync(userId.Value, request.ReceiverId);
                return Ok(friendship);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("accept/{friendshipId}")]
        public async Task<IActionResult> AcceptFriendRequest(Guid friendshipId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _friendshipService.AcceptFriendRequestAsync(friendshipId, userId.Value);
            return success ? Ok() : BadRequest("Cannot accept friend request");
        }

        [HttpPost("decline/{friendshipId}")]
        public async Task<IActionResult> DeclineFriendRequest(Guid friendshipId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _friendshipService.DeclineFriendRequestAsync(friendshipId, userId.Value);
            return success ? Ok() : BadRequest("Cannot decline friend request");
        }

        [HttpDelete("{friendId}")]
        public async Task<IActionResult> RemoveFriend(Guid friendId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _friendshipService.RemoveFriendAsync(userId.Value, friendId);
            return success ? Ok() : BadRequest("Cannot remove friend");
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var friends = await _friendshipService.GetFriendsAsync(userId.Value);
            return Ok(friends);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var requests = await _friendshipService.GetPendingRequestsAsync(userId.Value);
            return Ok(requests);
        }

        [HttpGet("status/{friendId}")]
        public async Task<IActionResult> GetFriendshipStatus(Guid friendId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var status = await _friendshipService.GetFriendshipStatusAsync(userId.Value, friendId);
            return Ok(new { Status = status?.ToString() });
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        }
    }
}

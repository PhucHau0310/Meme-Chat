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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(string query, int take = 20)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var users = await _userService.SearchUsersAsync(query, userId.Value, take);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            // Return public user info only
            var publicUser = new
            {
                user.Id,
                user.Name,
                user.Avatar,
                user.IsOnline,
                user.LastSeen
            };

            return Ok(publicUser);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null) return NotFound();

            user.Name = request.Name ?? user.Name;
            user.Avatar = request.Avatar ?? user.Avatar;

            await _userService.UpdateUserAsync(user);
            return Ok(user);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        }
    }
}

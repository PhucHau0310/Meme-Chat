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
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IFileService _fileService;

        public MessagesController(IMessageService messageService, IFileService fileService)
        {
            _messageService = messageService;
            _fileService = fileService;
        }

        [HttpGet("private/{receiverId}")]
        public async Task<IActionResult> GetPrivateMessages(Guid receiverId, int skip = 0, int take = 50)
        {
            var messages = await _messageService.GetMessagesAsync(receiverId, null, skip, take);
            return Ok(messages);
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetGroupMessages(Guid groupId, int skip = 0, int take = 50)
        {
            var messages = await _messageService.GetMessagesAsync(null, groupId, skip, take);
            return Ok(messages);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(Guid messageId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            var attachment = await _fileService.SaveFileAsync(messageId, file);
            return Ok(attachment);
        }

        [HttpGet("download/{attachmentId}")]
        public async Task<IActionResult> DownloadFile(Guid attachmentId)
        {
            var filePath = await _fileService.GetFilePathAsync(attachmentId);
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/octet-stream", fileName);
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _messageService.DeleteMessageAsync(messageId, userId.Value);
            return success ? Ok() : BadRequest("Cannot delete message");
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        }
    }
}

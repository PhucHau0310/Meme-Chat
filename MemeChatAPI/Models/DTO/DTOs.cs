namespace MemeChatAPI.Models.DTO
{
    public class UpdateProfileRequest
    {
        public string? Name { get; set; }
        public string? Avatar { get; set; }
    }

    public class SendFriendRequestModel
    {
        public Guid ReceiverId { get; set; }
    }

    public class CreateGroupRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPrivate { get; set; } = false;
    }

    public class AddMemberRequest
    {
        public Guid UserId { get; set; }
    }
}

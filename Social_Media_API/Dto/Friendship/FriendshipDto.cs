using Social_Media_API.Model;

namespace Social_Media_API.Dto.Friendship
{
    public class FriendshipDto
    {
        public int FriendshipId { get; set; }
        public string RequesterId { get; set; }
        public string ReceiverId { get; set; }
        public string RequesterName { get; set; }
        public string ReceiverName { get; set; }
        public FriendshipStatus Status { get; set; }
    }
}

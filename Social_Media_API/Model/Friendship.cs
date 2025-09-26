namespace Social_Media_API.Model
{
    public enum FriendshipStatus
    {
        Pending=0,
        Accepted=1,
        Rejected=2
    }
    public class Friendship
    {
        public int FriendshipId { get; set; }

  
        public string RequesterId { get; set; }
        public string ReceiverId { get; set; }

      
        public virtual User Requester { get; set; }
        public virtual User Receiver { get; set; }

        public FriendshipStatus Status { get; set; }
    }

    }



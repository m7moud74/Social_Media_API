using Social_Media_API.Model;

namespace Social_Media_API.Reposatory
{
    public interface IFriendshipRpo:IGenaricRepo<Friendship>
    {
        bool Exists(string requesterId, string receiverId);
        IEnumerable<Friendship> GetPendingRequests(string userId);
        IEnumerable<Friendship> GetFriends(string userId);
    }
}

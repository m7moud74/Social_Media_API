using Social_Media_API.Model;
using Social_Media_API.Reposatory.Genric_Repo;

namespace Social_Media_API.Reposatory.FriendShip_Repo
{
    public interface IFriendshipRpo:IGenaricRepo<Friendship>
    {
        bool Exists(string requesterId, string receiverId);
        IEnumerable<Friendship> GetPendingRequests(string userId);
        IEnumerable<Friendship> GetFriends(string userId);
    }
}

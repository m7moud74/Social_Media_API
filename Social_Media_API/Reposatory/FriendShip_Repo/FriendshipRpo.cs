using Microsoft.EntityFrameworkCore;
using Social_Media_API.Data;
using Social_Media_API.Model;
using Social_Media_API.Reposatory.Genric_Repo;

namespace Social_Media_API.Reposatory.FriendShip_Repo
{
    public class FriendshipRpo:GenaricRepo<Friendship>,IFriendshipRpo
    {
        private readonly AppDbContext _context;

        public FriendshipRpo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public bool Exists(string requesterId, string receiverId)
        {
            return _context.Friendships.Any(f =>
                f.RequesterId == requesterId && f.ReceiverId == receiverId ||
                f.RequesterId == receiverId && f.ReceiverId == requesterId);
        }

        public IEnumerable<Friendship> GetPendingRequests(string userId)
        {
            return _context.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending)
                .ToList();
        }

        public IEnumerable<Friendship> GetFriends(string userId)
        {
            return _context.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) && f.Status == FriendshipStatus.Accepted)
                .ToList();
        }
    }
}


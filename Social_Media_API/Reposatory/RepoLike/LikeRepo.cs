using Microsoft.EntityFrameworkCore;
using Social_Media_API.Data;
using Social_Media_API.Model;

namespace Social_Media_API.Reposatory.RepoLike
{
    public class LikeRepo : GenaricRepo<Like>, ILikeRepo
    {
        private readonly AppDbContext con;

        public LikeRepo(AppDbContext con) : base(con)
        {
            this.con = con;
        }

        public IEnumerable<Like> GetWithIncludes()
        {
           return con.Likes.Include(u=>u.User).Include(p=>p.Post).ToList();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Social_Media_API.Data;
using Social_Media_API.Model;
using Social_Media_API.Reposatory.Genric_Repo;

namespace Social_Media_API.Reposatory.Like_Repo
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

using Microsoft.EntityFrameworkCore;
using Social_Media_API.Data;
using Social_Media_API.Model;

namespace Social_Media_API.Reposatory.PostRepo
{
    public class PostRepo : GenaricRepo<Post>,IPostRepo
    {
        private readonly AppDbContext con;

        public PostRepo(AppDbContext con):base(con)
        {
            this.con = con;
        }
        public IEnumerable<Post> GetWithIncludes()
        {
            return con.Posts
        .Include(p => p.User)
        .Include(p => p.Comments)
            .ThenInclude(c => c.User)
        .Include(p => p.Likes)
            .ThenInclude(l => l.User)
        .ToList();
        }
    }
}

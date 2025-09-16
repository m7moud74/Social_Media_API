using Microsoft.EntityFrameworkCore;
using Social_Media_API.Data;
using Social_Media_API.Model;

namespace Social_Media_API.Reposatory
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
            return con.Posts.Include(u => u.User).Include(c => c.Comments).Include(L => L.Likes).ToList();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Social_Media_API.Data;
using Social_Media_API.Model;

namespace Social_Media_API.Reposatory.RpoComment
{
    public class CommentRepo : GenaricRepo<Comment>, ICommentRepo
    {
        private readonly AppDbContext con;

        public CommentRepo(AppDbContext con) : base(con)
        {
            this.con = con;
        }

        IEnumerable<Comment> ICommentRepo.GetWithInclude()
        {
            return con.Comments.Include(u => u.User).Include(p => p.Post).ToList();
        }
    }
}

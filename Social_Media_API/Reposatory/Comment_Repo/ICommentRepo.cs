using Social_Media_API.Model;
using Social_Media_API.Reposatory.Genric_Repo;

namespace Social_Media_API.Reposatory.Comment_Repo
{
    public interface ICommentRepo:IGenaricRepo<Comment>
    {
        public IEnumerable<Comment> GetWithInclude();
        
    }
}

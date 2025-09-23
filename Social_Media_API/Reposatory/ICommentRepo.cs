using Social_Media_API.Model;

namespace Social_Media_API.Reposatory
{
    public interface ICommentRepo:IGenaricRepo<Comment>
    {
        public IEnumerable<Comment> GetWithInclude();
        
    }
}

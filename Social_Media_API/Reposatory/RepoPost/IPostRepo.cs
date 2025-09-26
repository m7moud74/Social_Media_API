using Social_Media_API.Model;

namespace Social_Media_API.Reposatory.PostRepo
{
    public interface IPostRepo:IGenaricRepo<Post>
    {
        public IEnumerable<Post> GetWithIncludes();
    }
}

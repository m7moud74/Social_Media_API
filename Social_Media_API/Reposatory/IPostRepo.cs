using Social_Media_API.Model;

namespace Social_Media_API.Reposatory
{
    public interface IPostRepo
    {
        public IEnumerable<Post> GetWithIncludes();
    }
}

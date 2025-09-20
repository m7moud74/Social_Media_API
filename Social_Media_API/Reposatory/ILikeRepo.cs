using Social_Media_API.Model;

namespace Social_Media_API.Reposatory
{
    public interface ILikeRepo
    {
        public IEnumerable<Like> GetWithIncludes();
    }
}

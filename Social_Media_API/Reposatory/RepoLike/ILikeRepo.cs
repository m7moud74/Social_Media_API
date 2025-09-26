using Social_Media_API.Model;

namespace Social_Media_API.Reposatory.RepoLike
{
    public interface ILikeRepo:IGenaricRepo<Like>
    {
        public IEnumerable<Like> GetWithIncludes();
    }
}

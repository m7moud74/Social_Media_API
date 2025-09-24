using Social_Media_API.Model;
using Social_Media_API.Reposatory.Genric_Repo;

namespace Social_Media_API.Reposatory.Like_Repo
{
    public interface ILikeRepo:IGenaricRepo<Like>
    {
        public IEnumerable<Like> GetWithIncludes();
    }
}

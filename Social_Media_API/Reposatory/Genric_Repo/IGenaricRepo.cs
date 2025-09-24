using Social_Media_API.Model;

namespace Social_Media_API.Reposatory.Genric_Repo
{
    public interface IGenaricRepo<T> where T : class
    {
        public IEnumerable<T> GetAll();
        public T GetById(int id);
        public void Create(T entity);
        public void Update(int? id, T entity);
        public void Delete(int id);
        public  void Save();
       
    }
}

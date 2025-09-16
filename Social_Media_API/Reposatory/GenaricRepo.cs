using Social_Media_API.Data;
using System.Dynamic;

namespace Social_Media_API.Reposatory
{
    public class GenaricRepo<T> : IGenaricRepo<T> where T : class
    {
        private readonly AppDbContext con;

        public GenaricRepo(AppDbContext con)
        {
            this.con = con;
        }

        public void Create(T entity)
        {
           con.Set<T>().Add(entity);
        }

        public void Delete(int id)
        {
            con.Set<T>().Remove(GetById(id));
        }

        public IEnumerable<T> GetAll()
        {
            return con.Set<T>().ToList();
        }

        public T GetById(int id)
        {
           return con.Set<T>().Find(id);
        }

        public void Save()
        {
            con.SaveChanges();
        }

        public void Update(int ?id, T entity)
        {
            var existingEntity = GetById((int)id);
            if (existingEntity != null&&id!=null)
            {
                con.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                con.Set<T>().Update(entity);
            }
        }
    }
}

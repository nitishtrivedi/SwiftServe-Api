using SwiftServe_API.Models;

namespace SwiftServe_API.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetById(int id);
        IQueryable<T> GetAll();
        Task Add(T entity);
        Task Save();
    }
}

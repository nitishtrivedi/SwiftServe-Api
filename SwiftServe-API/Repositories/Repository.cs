using Microsoft.EntityFrameworkCore;
using SwiftServe_API.Data;
using SwiftServe_API.Models;

namespace SwiftServe_API.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _db;

        public Repository(AppDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<T> GetById(int id)
        {
            return await _db.FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<T> GetAll()
        {
            return _db.AsQueryable();
        }

        public async Task Add(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}

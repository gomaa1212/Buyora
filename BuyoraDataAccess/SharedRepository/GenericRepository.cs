
using BuyoraDataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace BuyoraDataAccess.SharedRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbset;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbset.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbset.AddRangeAsync(entities);
        }

        public void Delete(T entity)
        {
             _dbset.Remove(entity);
        }

        public async Task<T> GeByIdAsync(int Id)
        {
            return await _dbset.FindAsync(Id);
        }

        public async Task<List<T>> GetAsListAsync()
        {
            return await _dbset.ToListAsync();
        }

        public IQueryable<T> GetAsQuerable()
        {
            return _dbset;
        }

        public void  Update(T entity)
        {
             _dbset.Update(entity);
        }
    }
}

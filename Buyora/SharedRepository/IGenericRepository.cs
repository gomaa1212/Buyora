namespace Buyora.SharedRepository
{
    public interface IGenericRepository<T> where T : class
    {
        public IQueryable<T> GetAsQuerable();
        public Task<T> GeByIdAsync(int Id);
        public Task<List<T>> GetAsListAsync();
        public Task AddAsync(T entity);
        public void DeleteAsync(T entity);
        public void UpdateAsync(T entity);
        public Task AddRangeAsync(IEnumerable<T> entities);
    }
}

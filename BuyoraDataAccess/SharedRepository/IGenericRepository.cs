namespace BuyoraDataAccess.SharedRepository
{
    public interface IGenericRepository<T> where T : class
    {
        public IQueryable<T> GetAsQuerable();
        public Task<T> GeByIdAsync(int Id);
        public Task<List<T>> GetAsListAsync();
        public Task AddAsync(T entity);
        public void Delete(T entity);
        public void Update(T entity);
        public Task AddRangeAsync(IEnumerable<T> entities);
    }
}

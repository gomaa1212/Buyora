
using BuyoraDataAccess.SharedRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuyoraDataAccess.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<T> Repository<T>() where T : class;
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public Task<int> SaveChangesAsync();
        public Task RollbackTransactionAsync();
    }
}

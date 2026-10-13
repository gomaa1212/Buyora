using Buyora.Data;
using Buyora.SharedRepository;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Storage;

namespace Buyora.UnitOfWorks
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

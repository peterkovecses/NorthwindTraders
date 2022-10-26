using Northwind.Application.Models;
using System.Linq.Expressions;

namespace Northwind.Application.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity, TId> where TEntity : class
    {
        Task<(int totalItems, IEnumerable<TEntity> items)> GetAsync(
            IPagination pagination, 
            Sorting sorting, 
            Expression<Func<TEntity, bool>>? predicate = default, 
            CancellationToken token = default);

        Task<TEntity>? FindByIdAsync(TId id, CancellationToken token = default);

        Task AddAsync(TEntity entity, CancellationToken token = default);

        void Remove(TEntity entity);
    }
}

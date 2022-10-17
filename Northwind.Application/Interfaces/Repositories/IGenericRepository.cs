using Northwind.Application.Models;
using System.Linq.Expressions;

namespace Northwind.Application.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity, TId> where TEntity : class
    {
        Task<RepositoryCollectionResult<TEntity>> GetAsync(
            Pagination? pagination = null, 
            Sorting? sorting = null, 
            Expression<Func<TEntity, bool>> ? predicate = null, 
            CancellationToken token = default);

        Task<TEntity>? FindByIdAsync(TId id, CancellationToken token = default);

        Task AddAsync(TEntity entity, CancellationToken token = default);

        void Remove(IEnumerable<TEntity> entities);
    }
}

using Northwind.Application.Models;

namespace Northwind.Application.Interfaces
{
    public interface IPaginationStrategy<TEntity> where TEntity : class
    {
        Task<RepositoryCollectionResult<TEntity>> GetItemsAsync(CancellationToken token);
    }
}

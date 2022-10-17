using Microsoft.EntityFrameworkCore;
using Northwind.Application.Interfaces;
using Northwind.Application.Models;

namespace Northwind.Infrastructure.Persistence.Strategies
{
    public class NoPaginationStrategy<TEntity> : IPaginationStrategy<TEntity> where TEntity : class
    {
        private readonly IQueryable<TEntity> _query;

        public NoPaginationStrategy(IQueryable<TEntity> query, Pagination? pagination = null)
        {
            _query = query;
        }

        public async Task<RepositoryCollectionResult<TEntity>> GetItemsAsync(CancellationToken token)
        {
            var nonPaginatedItems = await _query.ToListAsync(token);
            return new RepositoryCollectionResult<TEntity>(nonPaginatedItems.Count, nonPaginatedItems);
        }
    }
}

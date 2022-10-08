using Microsoft.EntityFrameworkCore;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;

namespace Northwind.Infrastructure.Strategies
{
    public class PaginationStrategy<TEntity> : IPaginationStrategy<TEntity> where TEntity : class
    {
        private readonly IQueryable<TEntity> _query;
        private readonly IPaginationQuery? _paginationQuery;

        public PaginationStrategy(IQueryable<TEntity> query, IPaginationQuery? paginationQuery = null)
        {
            _query = query;
            _paginationQuery = paginationQuery;
        }

        public async Task<(int, IEnumerable<TEntity>)> GetItemsAsync()
        {
            var totalItems = await _query.CountAsync();
            var items = await Paginate(_paginationQuery, _query, totalItems);

            return (totalItems, items);
        }

        private static async Task<IEnumerable<TEntity>> Paginate(IPaginationQuery? paginationFilter, IQueryable<TEntity> query, int totalItems)
        {
            if (totalItems > 0)
            {
                paginationFilter.SetValues(totalItems);

                return await query
                .Skip(paginationFilter.GetItemsToSkip())
                .Take(paginationFilter.GetItemsToTake(totalItems))
                .ToListAsync();
            }
            else
            {
                return new List<TEntity>();
            }
        }
    }
}

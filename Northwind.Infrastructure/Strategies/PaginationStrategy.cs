using Microsoft.EntityFrameworkCore;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Models;

namespace Northwind.Infrastructure.Strategies
{
    public class PaginationStrategy<TEntity> : IPaginationStrategy<TEntity> where TEntity : class
    {
        private readonly IQueryable<TEntity> _query;
        private readonly Pagination? _paginationQuery;

        public PaginationStrategy(IQueryable<TEntity> query, Pagination? pagination = null)
        {
            _query = query;
            _paginationQuery = pagination;
        }

        public async Task<(int, IEnumerable<TEntity>)> GetItemsAsync()
        {
            var totalItems = await _query.CountAsync();
            var items = await Paginate(_paginationQuery, _query, totalItems);

            return (totalItems, items);
        }

        private static async Task<IEnumerable<TEntity>> Paginate(Pagination? pagination, IQueryable<TEntity> query, int totalItems)
        {
            if (totalItems > 0)
            {
                return await query
                .Skip(pagination.GetItemsToSkip())
                .Take(pagination.GetItemsToTake(totalItems))
                .ToListAsync();
            }
            else
            {
                return new List<TEntity>();
            }
        }
    }
}

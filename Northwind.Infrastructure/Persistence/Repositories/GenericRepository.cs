using Microsoft.EntityFrameworkCore;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Repositories;
using System.Linq.Expressions;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public abstract class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : class
    {
        protected readonly DbContext _context;
        private readonly IStrategyResolver _strategyResolver;

        public GenericRepository(DbContext context, IStrategyResolver strategyResolver)
        {
            _context = context;
            _strategyResolver = strategyResolver;
        }

        public async Task<(int totalItems, IEnumerable<TEntity> items)> GetAsync(IPaginationQuery? paginationQuery = null, Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = _context.Set<TEntity>().ApplyFilter<TEntity>(predicate);
            var strategy = _strategyResolver.GetStrategy(query, paginationQuery);

            return await strategy.GetItemsAsync();
        }

        public virtual async Task<TEntity>? FindByIdAsync(TId id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public void Remove(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }
    }
}

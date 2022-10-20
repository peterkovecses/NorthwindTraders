using Microsoft.EntityFrameworkCore;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Repositories;
using Northwind.Application.Models;
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

        public async Task<(int totalItems, IEnumerable<TEntity> items)> GetAsync(
            Pagination? pagination = null, 
            Sorting? sorting = null, 
            Expression<Func<TEntity, bool>>? predicate = null, 
            CancellationToken token = default)
        {
            var query = _context.Set<TEntity>().ApplyFilter<TEntity>(predicate).OrderByCustom(sorting);
            var strategy = _strategyResolver.GetStrategy(query, pagination);

            return await strategy.GetItemsAsync(token);
        }

        public virtual async Task<TEntity>? FindByIdAsync(TId id, CancellationToken token = default)
        {
            return await _context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken: token);
        }

        public async Task AddAsync(TEntity entity, CancellationToken token = default)
        {
            await _context.Set<TEntity>().AddAsync(entity, token);
        }

        public void Remove(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }
    }
}

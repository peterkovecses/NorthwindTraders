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

        public GenericRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<(int totalItems, IEnumerable<TEntity> items)> GetAsync(
            IPagination pagination, 
            Sorting sorting, 
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken token = default)
        {
            var query = _context.Set<TEntity>().Where(predicate).OrderByCustom(sorting);
            var totalItems = await query.CountAsync(token);
            var items = await query.Paginate(pagination, totalItems, token);

            return (totalItems, items);
        }

        public virtual async Task<TEntity>? FindByIdAsync(TId id, CancellationToken token = default)
        {
            return await _context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken: token);
        }

        public async Task AddAsync(TEntity entity, CancellationToken token = default)
        {
            await _context.Set<TEntity>().AddAsync(entity, token);
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
    }
}

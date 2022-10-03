using Microsoft.EntityFrameworkCore;
using Northwind.Domain.Common.Interfaces.Repositories;
using Northwind.Domain.Common.Queries;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public abstract class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : class
    {
        protected readonly DbContext _context;

        public GenericRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<(int totalItems, IEnumerable<TEntity> items)> GetAllAsync(PaginationFilter paginationFilter)
        {
            int toSkip = GetNumberOfItemsToSkip(paginationFilter);
            var query = _context.Set<TEntity>().AsQueryable();

            var totalItemsTask = query.CountAsync();

            var itemsTask = query
                .Skip(toSkip)
                .Take(paginationFilter.PageSize);

            return (await totalItemsTask, await itemsTask.ToListAsync());
        }

        public virtual async Task<TEntity>? GetAsync(TId id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(
            System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, 
            PaginationFilter? paginationFilter = null)
        {
            if (paginationFilter == null)
            {
                return await _context.Set<TEntity>().Where(predicate).ToListAsync();
            }

            var skip = GetNumberOfItemsToSkip(paginationFilter);

            return await _context.Set<TEntity>().Where(predicate).Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
        }

        public async Task<TEntity?> FindSingleOrDefaultAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        private static int GetNumberOfItemsToSkip(PaginationFilter? paginationFilter)
        {
            return (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
        }
    }
}

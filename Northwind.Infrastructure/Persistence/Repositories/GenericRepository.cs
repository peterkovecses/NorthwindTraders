using Microsoft.EntityFrameworkCore;
using Northwind.Domain.Common.Interfaces.Repositories;
using System.Linq.Expressions;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;

        public GenericRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity>? GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().Where(predicate).ToListAsync();
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
    }
}

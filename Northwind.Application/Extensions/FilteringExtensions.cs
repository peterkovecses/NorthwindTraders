using System.Linq.Expressions;

namespace Northwind.Application.Extensions
{
    public static class FilteringExtensions
    {
        public static IQueryable<TEntity> ApplyFilter<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate) 
        {
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }
    }
}

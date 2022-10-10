using Northwind.Application.Models;
using System.Linq.Expressions;

namespace Northwind.Application.Extensions
{
    public static class SortingExtensions
    {
        public static IQueryable<T> OrderByCustom<T>(this IQueryable<T> items, Sorting sorting)
        {
            if (sorting == null || sorting.SortBy == null)
            {
                return items;
            }

            var type = typeof(T);
            var expression2 = Expression.Parameter(type, "t");
            var property = type.GetProperty(sorting.SortBy);

            if (property == null)
            {
                return items;
            }

            var expression1 = Expression.MakeMemberAccess(expression2, property);
            var lambda = Expression.Lambda(expression1, expression2);
            var result = Expression.Call(
                typeof(Queryable),
                sorting.DescendingOrder ? "OrderByDescending" : "OrderBy",
                new Type[] { type, property.PropertyType },
                items.Expression,
                Expression.Quote(lambda));

            return items.Provider.CreateQuery<T>(result);
        }
    }
}

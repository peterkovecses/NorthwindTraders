using Northwind.Application.Exceptions;
using Northwind.Application.Models;
using System.Linq.Expressions;

namespace Northwind.Application.Extensions
{
    public static class SortingExtensions
    {
        public static IQueryable<T> OrderByCustom<T>(this IQueryable<T> items, Sorting sorting)
        {
            if (sorting.IsNoSorting)
            {
                return items;
            }

            var type = typeof(T);
            var parameterExpression = Expression.Parameter(type, "t");
            
            var property = type.GetProperty(sorting.SortBy);
            if (property == null)
            {
                throw new PropertyNotFoundException(sorting.SortBy);
            }

            var memberExpression = Expression.MakeMemberAccess(parameterExpression, property);
            var lambdaExpression = Expression.Lambda(memberExpression, parameterExpression);
            var resultExpression = Expression.Call(
                typeof(Queryable),
                sorting.DescendingOrder ? "OrderByDescending" : "OrderBy",
                new Type[] { type, property.PropertyType },
                items.Expression,
                Expression.Quote(lambdaExpression));

            return items.Provider.CreateQuery<T>(resultExpression);
        }
    } 
}

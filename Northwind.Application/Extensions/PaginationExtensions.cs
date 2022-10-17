using Northwind.Application.Interfaces;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;

namespace Northwind.Application.Extensions
{
    public static class PaginationExtensions
    {
        public static int GetItemsToSkip(this Pagination paginationQuery)
        {
            return (paginationQuery.PageNumber - 1) * paginationQuery.PageSize;
        }

        public static int GetItemsToTake(this Pagination paginationQuery, int totalItems)
        {
            if (paginationQuery.PageSize == 0)
            {
                return totalItems;
            }

            return paginationQuery.PageSize;
        }

        public static QueryParameters<TFilter> SetPaginationIfNull<TFilter>(this QueryParameters<TFilter> queryParameters, int totalItems) where TFilter : IFilter
        {
            if (queryParameters.Pagination == null)
            {
                queryParameters.Pagination = new Pagination
                {
                    PageNumber = 1,
                    PageSize = totalItems
                };
            }

            return queryParameters;
        }
    }
}

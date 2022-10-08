using Northwind.Application.Models;
using Northwind.Application.Models.Queries;

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

        public static QueryParameters SetPaginationIfNull(this QueryParameters queryParameters, int totalItems)
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

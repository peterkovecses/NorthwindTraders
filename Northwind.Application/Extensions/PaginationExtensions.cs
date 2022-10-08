using Northwind.Application.Interfaces;

namespace Northwind.Application.Extensions
{
    public static class PaginationExtensions
    {
        public static IPaginationQuery SetValues(this IPaginationQuery paginationQuery, int totalItems)
        {
            if (paginationQuery.PageNumber < 1)
            {
                paginationQuery.PageNumber = 1;
                paginationQuery.PageSize = totalItems;
            }

            return paginationQuery;
        }

        public static int GetItemsToSkip(this IPaginationQuery paginationQuery)
        {
            return (paginationQuery.PageNumber - 1) * paginationQuery.PageSize;
        }

        public static int GetItemsToTake(this IPaginationQuery paginationQuery, int totalItems)
        {
            if (paginationQuery.PageSize == 0)
            {
                return totalItems;
            }

            return paginationQuery.PageSize;
        }
    }
}

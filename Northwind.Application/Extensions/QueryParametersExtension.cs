using Northwind.Application.Interfaces;
using Northwind.Application.Models;

namespace Northwind.Application.Extensions
{
    public static class QueryParametersExtension
    {
        public static void SetParameters<T>(this QueryParameters<T> queryParameters, string defaultSortBy) where T : IFilter
        {
            queryParameters.Pagination ??= new NoPagination();

            if (queryParameters.Sorting == null)
            {
                queryParameters.Sorting = new Sorting { SortBy = defaultSortBy };
            }
            else if (string.IsNullOrEmpty(queryParameters.Sorting.SortBy))
            {
                queryParameters.Sorting.SortBy = defaultSortBy;
            }            
        }
    }
}

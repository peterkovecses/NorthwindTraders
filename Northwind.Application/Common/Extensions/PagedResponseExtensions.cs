using Northwind.Application.Common.Queries;
using Northwind.Application.Common.Responses;

namespace Northwind.Application.Common.Extensions
{
    public static class PagedResponseExtensions
    {
        public static PagedResponse<T> SetPagination<T>(
            this PagedResponse<T> pagedResponse,
            PaginationQuery paginationQuery,
            string nextPageUri,
            string previousPageUri,
            int totalItems)
        {
            pagedResponse.PageNumber = paginationQuery.PageNumber;
            pagedResponse.PageSize = paginationQuery.PageSize;
            pagedResponse.TotalItems = totalItems;
            pagedResponse.NextPage = pagedResponse.TotalPages > pagedResponse.PageNumber ? nextPageUri : null;
            pagedResponse.PreviousPage = previousPageUri;
            
            return pagedResponse;
        }
    }
}

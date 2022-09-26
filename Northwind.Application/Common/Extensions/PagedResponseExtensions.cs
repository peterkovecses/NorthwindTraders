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
            string previousPageUri)
        {
            pagedResponse.PageNumber = paginationQuery.PageNumber >= 1 ? paginationQuery.PageNumber : null;
            pagedResponse.PageSize = paginationQuery.PageSize >= 1 ? paginationQuery.PageSize : null;
            pagedResponse.NextPage = pagedResponse.Data.Any() && pagedResponse.Data.Count() == paginationQuery.PageSize ? nextPageUri : null;
            pagedResponse.PreviousPage = previousPageUri;
            
            return pagedResponse;
        }
    }
}

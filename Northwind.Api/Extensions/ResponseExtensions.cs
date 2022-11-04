using Microsoft.AspNetCore.WebUtilities;
using Northwind.Application.Interfaces;
using Northwind.Application.Models;

namespace Northwind.Api.Extensions
{
    public static class ResponseExtensions
    {
        public static PagedResponse<T> SetNavigation<T>(this PagedResponse<T> response, string baseUri)
        {
            if (response.PageNumber >= 1)
            {
                response.NextPage = GetPaginatedUri(baseUri, new Pagination { PageNumber = response.PageNumber + 1, PageSize = response.PageSize });
            }

            if (response.PageNumber - 1 >= 1)
            {
                response.PreviousPage = GetPaginatedUri(baseUri, new Pagination { PageNumber = response.PageNumber - 1, PageSize = response.PageSize });
            }

            return response;
        }

        private static string GetPaginatedUri(string baseUri, IPagination paginationQuery)
        {
            var uri = new Uri(baseUri);

            var modifiedUri = QueryHelpers.AddQueryString(baseUri, "Pagination.PageNumber", paginationQuery.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "Pagination.PageSize", paginationQuery.PageSize.ToString());

            return new Uri(modifiedUri).ToString();
        }
    }
}

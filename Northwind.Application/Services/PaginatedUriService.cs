using Microsoft.AspNetCore.WebUtilities;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;

namespace Northwind.Application.Services
{
    public class PaginatedUriService : IPaginatedUriService
    {
        private string _baseUri;

        public PaginatedUriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public (string? next, string? previous) GetNavigations(IPaginationQuery paginationQuery)
        {
            string? next = default;
            string? previous = default;

            if (paginationQuery.PageNumber >= 1)
            {
                next = GetPaginatedUri(_baseUri, new PaginationQuery { PageNumber = paginationQuery.PageNumber + 1, PageSize = paginationQuery.PageSize });
            }

            if (paginationQuery.PageNumber - 1 >= 1)
            {
                previous = GetPaginatedUri(_baseUri, new PaginationQuery { PageNumber = paginationQuery.PageNumber - 1, PageSize = paginationQuery.PageSize });
            }

            return (next, previous);
        }

        private static string GetPaginatedUri(string baseUri, PaginationQuery paginationQuery)
        {
            var uri = new Uri(baseUri);

            var modifiedUri = QueryHelpers.AddQueryString(baseUri, "pageNumber", paginationQuery.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", paginationQuery.PageSize.ToString());

            return new Uri(modifiedUri).ToString();
        }
    }
}

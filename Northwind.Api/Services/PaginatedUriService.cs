using Microsoft.AspNetCore.WebUtilities;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;

namespace Northwind.Api.Services
{
    public class PaginatedUriService : IPaginatedUriService
    {
        private readonly string _baseUri;

        public PaginatedUriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public (string? next, string? previous) GetNavigations(IPagination pagination)
        {
            string? next = default;
            string? previous = default;

            if (pagination.PageNumber >= 1)
            {
                next = GetPaginatedUri(_baseUri, new Pagination { PageNumber = pagination.PageNumber + 1, PageSize = pagination.PageSize });
            }

            if (pagination.PageNumber - 1 >= 1)
            {
                previous = GetPaginatedUri(_baseUri, new Pagination { PageNumber = pagination.PageNumber - 1, PageSize = pagination.PageSize });
            }

            return (next, previous);
        }

        private static string GetPaginatedUri(string baseUri, IPagination paginationQuery)
        {
            var uri = new Uri(baseUri);

            var modifiedUri = QueryHelpers.AddQueryString(baseUri, "pageNumber", paginationQuery.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", paginationQuery.PageSize.ToString());

            return new Uri(modifiedUri).ToString();
        }
    }
}

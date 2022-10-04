using Northwind.Application.Common.Queries;
using Northwind.Application.Common.Responses;

namespace Northwind.Application.Common.Extensions
{
    public static class ResponseHelper
    {
        public static Response<T> ToResponse<T>(this T data)
        {
            return new Response<T>(data);
        }

        public static Response<IEnumerable<T>> ToResponse<T>(this IEnumerable<T> data)
        {
            return new Response<IEnumerable<T>>(data);
        }

        public static PagedResponse<T> ToPagedResponse<T>(
            this IEnumerable<T> data,
            PaginationQuery paginationQuery,
            int totalItems,
            string nextPageUri,
            string previousPageUri)
        {
            return new PagedResponse<T>(data, paginationQuery, totalItems, nextPageUri, previousPageUri);
        }
    }
}

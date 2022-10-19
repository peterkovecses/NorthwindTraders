using Northwind.Application.Models;

namespace Northwind.Application.Extensions
{
    public static class ResponseExtensions
    {
        public static Response<T> ToResponse<T>(this T data)
        {
            return new Response<T>(data);
        }

        public static PagedResponse<T> ToPagedResponse<T>(
            this IEnumerable<T> data,
            Pagination pagination,
            int totalItems,
            string nextPageUri,
            string previousPageUri)
        {
            return new PagedResponse<T>(data, pagination, totalItems, nextPageUri, previousPageUri);
        }
    }
}

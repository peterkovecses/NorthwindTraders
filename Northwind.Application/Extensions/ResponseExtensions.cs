using Northwind.Application.Interfaces;
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
            IPagination pagination,
            int totalItems)
        {
            return new PagedResponse<T>(data, pagination, totalItems);
        }
    }
}

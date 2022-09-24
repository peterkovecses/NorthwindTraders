using Northwind.Application.Common.Responses;

namespace Northwind.Application.Common.Extensions
{
    public static class ResponseConverter
    {
        public static Response<T> ToResponse<T>(this T obj)
        {
            return new Response<T>(obj);
        }

        public static Response<IEnumerable<T>> ToResponse<T>(this IEnumerable<T> objects)
        {
            return new Response<IEnumerable<T>>(objects);
        }

        public static PagedResponse<T> ToPagedResponse<T>(this IEnumerable<T> objects)
        {
            return new PagedResponse<T>(objects);
        }
    }
}

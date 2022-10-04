using Northwind.Application.Common.Queries;

namespace Northwind.Application.Common.Responses
{
    public class PagedResponse<T>
    {
        public PagedResponse(
            IEnumerable<T> data,
            PaginationQuery paginationQuery,
            int totalItems,
            string nextPageUri,
            string previousPageUri)
        {
            Data = data;
            PageNumber = paginationQuery.PageNumber;
            PageSize = paginationQuery.PageSize;
            TotalItems = totalItems;
            NextPage = TotalPages > PageNumber ? nextPageUri : null;
            PreviousPage = previousPageUri;
        }

        public IEnumerable<T> Data { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalItems { get; }
        public int TotalPages
        {
            get 
            { 
                return (int)Math.Ceiling((double)TotalItems / PageSize); 
            }
        }
        public string? NextPage { get; }
        public string? PreviousPage { get; }
    }
}

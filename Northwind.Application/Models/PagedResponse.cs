using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class PagedResponse<T>
    {
        public PagedResponse(
            IEnumerable<T> data,
            IPaginationQuery paginationQuery,
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
                return TotalItems == 0 ? 1 : (int)Math.Ceiling((double)TotalItems / PageSize);
            }
        }
        public string? NextPage { get; }
        public string? PreviousPage { get; }
    }
}

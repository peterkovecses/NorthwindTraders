using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class PagedResponse<T>
    {
        private string? _nextPage;

        public PagedResponse(
                    IEnumerable<T> data,
                    IPagination pagination,
                    int totalItems)
        {
            Data = data;
            PageNumber = pagination.PageNumber;
            PageSize = pagination.PageSize;
            TotalItems = totalItems;
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
        public string? NextPage { get => _nextPage; set => _nextPage = TotalPages > PageNumber ? value : null; }
        public string? PreviousPage { get; set; }
        public bool HasData => Data != null && Data.Any();
    }
}

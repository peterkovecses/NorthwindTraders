namespace Northwind.Application.Common.Responses
{
    public class PagedResponse<T>
    {
        public PagedResponse(IEnumerable<T> data)
        {
            Data = data;
        }

        public IEnumerable<T> Data { get; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? NextPage { get; set; }
        public string? PreviousPage { get; set; }
    }
}

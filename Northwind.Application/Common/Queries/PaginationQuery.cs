namespace Northwind.Application.Common.Queries
{
    public class PaginationQuery
    {
        private const int MaxPageSize = 100;

        public PaginationQuery()
        {
            PageNumber = 1;
            PageSize = MaxPageSize;
        }

        public PaginationQuery(int pageBumber, int pageSize)
        {
            PageNumber = pageBumber;
            PageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

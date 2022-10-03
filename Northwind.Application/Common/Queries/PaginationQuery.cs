namespace Northwind.Application.Common.Queries
{
    public class PaginationQuery
    {
        private const int MinPageNumber = 1;

        private const int MinPageSize = 1;

        private const int MaxPageSize = 100;

        public PaginationQuery()
        {
            PageNumber = MinPageNumber;
            PageSize = MaxPageSize;
        }

        public PaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < MinPageNumber ? MinPageNumber : pageNumber;
            PageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize < MinPageSize ? MinPageSize : pageSize;
        }

        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }

    }
}

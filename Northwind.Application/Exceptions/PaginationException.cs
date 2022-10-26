namespace Northwind.Application.Exceptions
{
    public class PaginationException : Exception
    {
        public int PageNumber { get; init; }
        public PaginationException(int pageNumber) : base($"Page number {pageNumber} does not exist.")
        {
            PageNumber = pageNumber;
        }
    }
}

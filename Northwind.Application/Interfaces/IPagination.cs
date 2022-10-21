namespace Northwind.Application.Interfaces
{
    public interface IPagination
    {
        int PageNumber { get; }
        int PageSize { get; }
    }
}
namespace Northwind.Application.Interfaces
{
    public interface ISorting
    {
        bool DescendingOrder { get; set; }
        string? SortBy { get; set; }
    }
}
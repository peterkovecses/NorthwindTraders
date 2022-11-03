namespace Northwind.Application.Models
{
    public class Sorting
    {
        public string? SortBy { get; init; } = "Created";
        public bool DescendingOrder { get; init; }
        public bool IsNoSorting { get; private init; }
        public static Sorting NoSorting => new() { SortBy = default, IsNoSorting = true };
    }
}

namespace Northwind.Application.Models
{
    public class Sorting
    {
        public string? SortBy { get; set; } = "Created";
        public bool DescendingOrder { get; set; }
        public bool IsNoSorting { get; private init; }
        public static Sorting NoSorting => new() { SortBy = default, IsNoSorting = true };
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Northwind.Application.Models
{
    public class Sorting
    {
        public string? SortBy { get; set; }

        public bool DescendingOrder { get; init; }

        [BindNever]
        public bool IsNoSorting => String.IsNullOrWhiteSpace(SortBy);

        public static Sorting NoSorting() => new();
    }
}

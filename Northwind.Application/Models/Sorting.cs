using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models
{
    public class Sorting
    {
        private string? _sortBy;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string? SortBy 
        { 
            get => _sortBy; 
            init => _sortBy = value != null && String.IsNullOrWhiteSpace(value) ? throw new ArgumentException(nameof(SortBy)) : value; 
        }

        public bool DescendingOrder { get; init; }

        [BindNever]
        public bool IsNoSorting => String.IsNullOrWhiteSpace(SortBy);

        public static Sorting NoSorting() => new();
    }
}

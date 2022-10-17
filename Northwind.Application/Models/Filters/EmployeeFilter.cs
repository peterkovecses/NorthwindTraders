using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class EmployeeFilter : IFilter
    {
        public string? SearchTerm { get; set; }
        public string? Title { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public DateTime? MinBirthDate { get; set; }
        public DateTime? MaxBirthDate { get; set; }
        public DateTime? MinHireDate { get; set; }
        public DateTime? MaxHireDate { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public int? ReportsTo { get; set; }
    }
}

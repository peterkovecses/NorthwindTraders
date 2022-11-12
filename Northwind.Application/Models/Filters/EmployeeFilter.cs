using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class EmployeeFilter : IFilter
    {
        public string? FullNameFraction { get; set; }
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

        public  ExpressionStarter<Employee> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Employee>(true);

            if (!string.IsNullOrEmpty(FullNameFraction))
            {
                var fragments = FullNameFraction.Split(' ');
                foreach (var fragment in fragments)
                {
                    predicate = predicate.And(e => e.FullName.ToLower().Contains(fragment.Trim().ToLower()));
                }
            }

            if (MinHireDate != null)
            {
                predicate = predicate.And(e => e.HireDate >= MinHireDate);
            }

            if (MinBirthDate != null)
            {
                predicate = predicate.And(e => e.BirthDate >= MinBirthDate);
            }

            if (MaxHireDate != null)
            {
                predicate = predicate.And(e => e.HireDate <= MaxHireDate);
            }

            if (MaxBirthDate != null)
            {
                predicate = predicate.And(e => e.BirthDate <= MaxBirthDate);
            }

            if (!string.IsNullOrEmpty(City))
            {
                predicate = predicate.And(e => e.City == City);
            }

            if (!string.IsNullOrEmpty(Country))
            {
                predicate = predicate.And(e => e.Country == Country);
            }

            if (!string.IsNullOrEmpty(PostalCode))
            {
                predicate = predicate.And(e => e.PostalCode == PostalCode);
            }

            if (!string.IsNullOrEmpty(Region))
            {
                predicate = predicate.And(e => e.Region == Region);
            }

            if (ReportsTo != null)
            {
                predicate = predicate.And(e => e.ReportsTo == ReportsTo);
            }

            if (!string.IsNullOrEmpty(Title))
            {
                predicate = predicate.And(e => e.Title == Title);
            }

            if (!string.IsNullOrEmpty(TitleOfCourtesy))
            {
                predicate = predicate.And(e => e.TitleOfCourtesy == TitleOfCourtesy);
            }

            return predicate;
        }
    }
}

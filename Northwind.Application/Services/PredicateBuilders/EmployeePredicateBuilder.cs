using LinqKit;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class EmployeePredicateBuilder
    {
        public virtual ExpressionStarter<Employee> GetPredicate(QueryParameters<EmployeeFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Employee>(true);
            var filter = queryParameters.Filter;

            if (!string.IsNullOrEmpty(filter.FullNameFraction))
            {
                var fragments  = filter.FullNameFraction.Split(' ');
                foreach (var fragment in fragments)
                {                    
                    predicate = predicate.And(e => e.FullName.ToLower().Contains(fragment.Trim().ToLower()));
                }
            }

            if (filter.MinHireDate != null)
            {
                predicate = predicate.And(e => e.HireDate >= filter.MinHireDate);
            }

            if (filter.MinBirthDate != null)
            {
                predicate = predicate.And(e => e.BirthDate >= filter.MinBirthDate);
            }

            if (filter.MaxHireDate != null)
            {
                predicate = predicate.And(e => e.HireDate <= filter.MaxHireDate);
            }

            if (filter.MaxBirthDate != null)
            {
                predicate = predicate.And(e => e.BirthDate <= filter.MaxBirthDate);
            }

            if (filter.City != null)
            {
                predicate = predicate.And(e => e.City == filter.City);
            }

            if (filter.Country != null)
            {
                predicate = predicate.And(e => e.Country == filter.Country);
            }

            if (filter.PostalCode != null)
            {
                predicate = predicate.And(e => e.PostalCode == filter.PostalCode);
            }

            if (filter.Region != null)
            {
                predicate = predicate.And(e => e.Region == filter.Region);
            }

            if (filter.ReportsTo != null)
            {
                predicate = predicate.And(e => e.ReportsTo == filter.ReportsTo);
            }

            if (filter.Title != null)
            {
                predicate = predicate.And(e => e.Title == filter.Title);
            }

            if (filter.TitleOfCourtesy != null)
            {
                predicate = predicate.And(e => e.TitleOfCourtesy == filter.TitleOfCourtesy);
            }

            return predicate;
        }
    }
}

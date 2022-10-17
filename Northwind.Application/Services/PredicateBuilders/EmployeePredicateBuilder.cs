using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;
using Northwind.Application.Interfaces.Services.PredicateBuilders;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class EmployeePredicateBuilder : IEmployeePredicateBuilder
    {
        public ExpressionStarter<Employee> GetPredicate(QueryParameters<EmployeeFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Employee>();
            var filter = queryParameters.Filter;

            if (filter.SearchTerm != null)
            {
                predicate = predicate.And(e => e.FirstName.Contains(filter.SearchTerm) || e.LastName.Contains(filter.SearchTerm));
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

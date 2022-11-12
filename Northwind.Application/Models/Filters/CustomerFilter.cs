using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class CustomerFilter : IFilter
    {
        public string? CompanyNameFraction { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public ExpressionStarter<Customer> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Customer>(true);

            if (!string.IsNullOrEmpty(CompanyNameFraction))
            {
                predicate = predicate.And(c => c.CompanyName.ToLower().Contains(CompanyNameFraction.ToLower()));
            }

            if (!string.IsNullOrEmpty(City))
            {
                predicate = predicate.And(c => c.City == City);
            }

            if (!string.IsNullOrEmpty(Region))
            {
                predicate = predicate.And(c => c.Region == Region);
            }

            if (!string.IsNullOrEmpty(PostalCode))
            {
                predicate = predicate.And(c => c.PostalCode == PostalCode);
            }

            if (!string.IsNullOrEmpty(Country))
            {
                predicate = predicate.And(c => c.Country == Country);
            }

            return predicate;
        }
    }
}

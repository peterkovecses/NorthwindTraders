using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class SupplierFilter : IFilter<Supplier>
    {
        public string? CompanyName { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public ExpressionStarter<Supplier> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Supplier>(true);

            if (!string.IsNullOrEmpty(CompanyName))
            {
                predicate = predicate.And(s => s.CompanyName == CompanyName);
            }

            if (!string.IsNullOrEmpty(City))
            {
                predicate = predicate.And(s => s.City == City);
            }

            if (!string.IsNullOrEmpty(Region))
            {
                predicate = predicate.And(s => s.Region == Region);
            }

            if (!string.IsNullOrEmpty(PostalCode))
            {
                predicate = predicate.And(s => s.PostalCode == PostalCode);
            }

            if (!string.IsNullOrEmpty(Country))
            {
                predicate = predicate.And(s => s.Country == Country);
            }

            return predicate;
        }
    }
}

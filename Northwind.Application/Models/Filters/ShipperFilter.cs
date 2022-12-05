using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class ShipperFilter : IFilter<Shipper>
    {
        public string? CompanyName { get; set; }

        public ExpressionStarter<Shipper> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Shipper>(true);

            if (!string.IsNullOrEmpty(CompanyName))
            {
                predicate = predicate.And(s => s.CompanyName == CompanyName);
            }

            return predicate;
        }
    }
}

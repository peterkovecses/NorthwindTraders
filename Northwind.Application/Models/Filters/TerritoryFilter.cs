using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class TerritoryFilter : IFilter
    {
        public string? TerritoryDescriptionFraction { get; set; }
        public int? RegionId { get; set; }

        public ExpressionStarter<Territory> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Territory>(true);

            if (!string.IsNullOrEmpty(TerritoryDescriptionFraction))
            {
                predicate = predicate.And(t => t.TerritoryDescription.ToLower().Contains(TerritoryDescriptionFraction.ToLower()));
            }

            if (RegionId != null)
            {
                predicate = predicate.And(r => r.RegionId == RegionId);
            }

            return predicate;
        }
    }
}

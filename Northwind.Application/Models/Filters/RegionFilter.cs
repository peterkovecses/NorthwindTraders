using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class RegionFilter : IFilter
    {
        public string RegionDescriptionFragment { get; set; } = null!;

        public ExpressionStarter<Region> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Region>(true);

            if (!string.IsNullOrEmpty(RegionDescriptionFragment))
            {
                predicate = predicate.And(r => r.RegionDescription.ToLower().Contains(RegionDescriptionFragment.ToLower()));
            }

            return predicate;
        }
    }
}

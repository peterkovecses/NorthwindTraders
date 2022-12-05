using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class CustomerDemographicFilter : IFilter<CustomerDemographic>
    {
        public ExpressionStarter<CustomerDemographic> GetPredicate() => PredicateBuilder.New<CustomerDemographic>(true);
    }
}

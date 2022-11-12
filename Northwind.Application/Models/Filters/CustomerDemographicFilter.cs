using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;
using System.Linq.Expressions;

namespace Northwind.Application.Models.Filters
{
    public class CustomerDemographicFilter : IFilter
    {
        public Expression<Func<CustomerDemographic, bool>> GetPredicate() => _ => true;
    }
}

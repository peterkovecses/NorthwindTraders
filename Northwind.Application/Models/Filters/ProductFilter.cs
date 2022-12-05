using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class ProductFilter : IFilter<Product>
    {
        public string? ProductNameFraction { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinUnitPrice { get; set; }
        public decimal? MaxUnitPrice { get; set; }

        public ExpressionStarter<Product> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Product>(true);

            if (!string.IsNullOrEmpty(ProductNameFraction))
            {
                predicate = predicate.And(p => p.ProductName.ToLower().Contains(ProductNameFraction.ToLower()));
            }

            if (SupplierId != null)
            {
                predicate = predicate.And(p => p.SupplierId == SupplierId);
            }

            if (CategoryId != null)
            {
                predicate = predicate.And(p => p.CategoryId == CategoryId);
            }

            if (MinUnitPrice != null)
            {
                predicate = predicate.And(p => p.UnitPrice >= MinUnitPrice);
            }

            if (MaxUnitPrice != null)
            {
                predicate = predicate.And(p => p.UnitPrice <= MaxUnitPrice);
            }

            return predicate;
        }
    }
}

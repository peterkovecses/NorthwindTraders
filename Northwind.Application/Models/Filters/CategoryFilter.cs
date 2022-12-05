using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class CategoryFilter<EntityBase> : IFilter<Category>
    {

        public string? CategoryNameFraction { get; set; }

        public ExpressionStarter<Category> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Category>(true);

            if (!string.IsNullOrEmpty(CategoryNameFraction))
            {
                predicate = predicate.And(c => c.CategoryName.ToLower().Contains(CategoryNameFraction.ToLower()));
            }

            return predicate;
        }
    }
}

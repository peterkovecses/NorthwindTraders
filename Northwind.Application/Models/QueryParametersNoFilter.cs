using Microsoft.AspNetCore.Mvc.ModelBinding;
using Northwind.Application.Models.Filters;

namespace Northwind.Application.Models
{
    public class QueryParametersNoFilter : QueryParameters<NoFilter>
    {
        [BindNever]
        public override NoFilter Filter { get; init; } = new NoFilter();
    }
}

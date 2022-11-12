using Microsoft.AspNetCore.Mvc.ModelBinding;
using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class QueryParametersNoFilterBinding<T> : QueryParameters<T> where T : IFilter, new()
    {
        [BindNever]
        public override T Filter { get; init; } = new();
    }
}

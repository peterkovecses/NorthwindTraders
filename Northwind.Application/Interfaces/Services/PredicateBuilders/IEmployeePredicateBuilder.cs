using LinqKit;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Services.PredicateBuilders
{
    public interface IEmployeePredicateBuilder
    {
        ExpressionStarter<Employee> GetPredicate(QueryParameters<EmployeeFilter> queryParameters);
    }
}
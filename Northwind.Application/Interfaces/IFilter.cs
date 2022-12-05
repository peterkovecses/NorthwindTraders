using LinqKit;
using Northwind.Domain.Common;

namespace Northwind.Application.Interfaces
{
    public interface IFilter<T> where T : EntityBase
    {
        ExpressionStarter<T> GetPredicate();
    }
}
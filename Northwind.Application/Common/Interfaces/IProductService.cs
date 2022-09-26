using Northwind.Application.Dtos;

namespace Northwind.Application.Common.Interfaces
{
    public interface IProductService : IGenericService<ProductDto, int>
    {
    }
}

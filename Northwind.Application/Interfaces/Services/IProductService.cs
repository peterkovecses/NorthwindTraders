using Northwind.Application.Dtos;

namespace Northwind.Application.Interfaces.Services
{
    public interface IProductService : IGenericService<ProductDto, int>
    {
    }
}

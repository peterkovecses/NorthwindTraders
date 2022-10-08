using Northwind.Application.Dtos;

namespace Northwind.Application.Interfaces.Services
{
    public interface ICategoryService : IGenericService<CategoryDto, int>
    {
    }
}

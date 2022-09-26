using Northwind.Application.Dtos;

namespace Northwind.Application.Common.Interfaces
{
    public interface ICategoryService : IGenericService<CategoryDto, int>
    {
    }
}

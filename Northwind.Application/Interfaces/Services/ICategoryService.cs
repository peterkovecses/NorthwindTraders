using Northwind.Application.Dtos;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Common;
using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Services
{
    public interface ICategoryService : IGenericService<CategoryDto, int, CategoryFilter>
    {
    }
}

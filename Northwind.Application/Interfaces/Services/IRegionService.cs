using Northwind.Application.Dtos;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Services
{
    public interface IRegionService : IGenericService<RegionDto, int, RegionFilter, Region>
    {
    }
}

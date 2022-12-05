using Northwind.Application.Dtos;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Services
{
    public interface IShipperService : IGenericService<ShipperDto, int, ShipperFilter, Shipper>
    {
    }
}

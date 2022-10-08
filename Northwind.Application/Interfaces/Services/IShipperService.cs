using Northwind.Application.Dtos;

namespace Northwind.Application.Interfaces.Services
{
    public interface IShipperService : IGenericService<ShipperDto, int>
    {
    }
}

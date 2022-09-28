using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class ShipperService : IShipperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShipperService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShipperDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var shippers = await _unitOfWork.Shippers.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<ShipperDto>>(shippers);
        }

        public async Task<ShipperDto>? GetAsync(int id)
        {
            var shipper = await _unitOfWork.Shippers.GetAsync(id);

            return _mapper.Map<ShipperDto>(shipper);
        }

        public async Task<int> CreateAsync(ShipperDto shipperDto)
        {
            var shipper = _mapper.Map<Shipper>(shipperDto);

            await _unitOfWork.Shippers.AddAsync(shipper);
            await _unitOfWork.CompleteAsync();

            return shipper.ShipperId;
        }

        public async Task UpdateAsync(ShipperDto shipperDto)
        {
            var shipperInDb = await _unitOfWork.Regions.GetAsync(shipperDto.ShipperId);

            _mapper.Map(shipperDto, shipperInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<ShipperDto> DeleteAsync(int id)
        {
            var shipper = await _unitOfWork.Shippers.GetAsync(id);

            _unitOfWork.Shippers.Remove(shipper);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ShipperDto>(shipper);
        }

        public async Task<IEnumerable<ShipperDto>> DeleteRangeAsync(int[] ids)
        {
            var shippers = await _unitOfWork.Shippers.FindAllAsync(s => ids.Contains(s.ShipperId));

            _unitOfWork.Shippers.RemoveRange(shippers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<ShipperDto>>(shippers);
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Shippers.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Shippers.FindAllAsync(s => ids.Contains(s.ShipperId))).Count() == ids.Length;
        }
    }
}

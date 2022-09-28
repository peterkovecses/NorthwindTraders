using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class TerritoryService : ITerritoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TerritoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TerritoryDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var territories = await _unitOfWork.Territories.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<TerritoryDto>>(territories);
        }

        public async Task<TerritoryDto>? GetAsync(string id)
        {
            var territory = await _unitOfWork.Territories.GetAsync(id);

            return _mapper.Map<TerritoryDto>(territory);
        }

        public async Task<string> CreateAsync(TerritoryDto territoryDto)
        {
            var territory = _mapper.Map<Territory>(territoryDto);

            await _unitOfWork.Territories.AddAsync(territory);
            await _unitOfWork.CompleteAsync();

            return territory.TerritoryId;
        }

        public async Task UpdateAsync(TerritoryDto territoryDto)
        {
            var territoryInDb = await _unitOfWork.Territories.GetAsync(territoryDto.TerritoryId);

            _mapper.Map(territoryDto, territoryInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<TerritoryDto> DeleteAsync(string id)
        {
            var territory = await _unitOfWork.Territories.GetAsync(id);

            _unitOfWork.Territories.Remove(territory);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TerritoryDto>(territory);
        }

        public async Task<IEnumerable<TerritoryDto>> DeleteRangeAsync(string[] ids)
        {
            var territories = await _unitOfWork.Territories.FindAllAsync(t => ids.Contains(t.TerritoryId));

            _unitOfWork.Territories.RemoveRange(territories);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<TerritoryDto>>(territories);
        }

        public async Task<bool> IsExists(string id)
        {
            return await _unitOfWork.Territories.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(string[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Territories.FindAllAsync(t => ids.Contains(t.TerritoryId))).Count() == ids.Length;
        }
    }
}

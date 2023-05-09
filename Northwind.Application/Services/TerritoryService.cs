using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
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

        public async Task<PagedResponse<TerritoryDto>> GetAsync(QueryParameters<TerritoryFilter, Territory> queryParameters, CancellationToken token = default)
        {
            var (totalTerritories, territories) = await _unitOfWork.Territories.GetAsync(queryParameters.Pagination, queryParameters.Sorting, queryParameters.Filter.GetPredicate(), token);

            return _mapper.Map<IEnumerable<TerritoryDto>>(territories)
                .ToPagedResponse(queryParameters.Pagination, totalTerritories);
        }

        public async Task<Response<TerritoryDto>> FindByIdAsync(string id, CancellationToken token = default)
        {
            var territory = await _unitOfWork.Territories.FindByIdAsync(id, token);

            return _mapper.Map<TerritoryDto>(territory).ToResponse();
        }

        public async Task<Response<TerritoryDto>> CreateAsync(TerritoryDto territoryDto, CancellationToken token = default)
        {
            var territory = _mapper.Map<Territory>(territoryDto);

            await _unitOfWork.Territories.AddAsync(territory, token);
            await _unitOfWork.CompleteAsync();

            territoryDto.TerritoryId = territory.TerritoryId;

            return territoryDto.ToResponse();
        }

        public async Task<Response<TerritoryDto>> UpdateAsync(TerritoryDto territoryDto, CancellationToken token = default)
        {
            var territoryInDb = 
                await _unitOfWork.Territories.FindByIdAsync(territoryDto.TerritoryId, token) ?? throw new ItemNotFoundException<string>(territoryDto.TerritoryId);
            territoryInDb.TerritoryDescription = territoryDto.TerritoryDescription;
            territoryInDb.RegionId = territoryDto.RegionId;
            await _unitOfWork.CompleteAsync();

            return territoryDto.ToResponse();
        }

        public async Task DeleteAsync(string id, CancellationToken token = default)
        {
            var territoryToRemove = await _unitOfWork.Territories.FindByIdAsync(id, token);
            _unitOfWork.Territories.Remove(territoryToRemove);

            await _unitOfWork.CompleteAsync();
        }
    }
}

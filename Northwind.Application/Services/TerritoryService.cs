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

        public async Task<PagedResponse<TerritoryDto>> GetAsync(QueryParameters<TerritoryFilter> queryParameters, CancellationToken token = default)
        {
            var (totalTerritories, territories) = await _unitOfWork.Territories.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);

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
                await _unitOfWork.Territories.FindByIdAsync(territoryDto.TerritoryId, token) ?? throw new ItemNotFoundException(territoryDto.TerritoryId);
            _mapper.Map(territoryDto, territoryInDb);
            await _unitOfWork.CompleteAsync();

            return territoryDto.ToResponse();
        }

        public async Task<Response<IEnumerable<TerritoryDto>>> DeleteAsync(string[] ids, CancellationToken token = default)
        {
            var territoriesToRemove = (await _unitOfWork.Territories.GetAsync(Pagination.NoPagination(), Sorting.NoSorting(), t => ids.Contains(t.TerritoryId), token)).items;

            foreach (var territory in territoriesToRemove)
            {
            _unitOfWork.Territories.Remove(territory);
            }
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<TerritoryDto>>(territoriesToRemove).ToResponse();
        }
    }
}

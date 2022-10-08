using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class TerritoryService : ITerritoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public TerritoryService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<TerritoryDto>> GetAsync(IPaginationQuery paginationQuery)
        {
            var (totalItems, territories) = await _unitOfWork.Territories.GetAsync(paginationQuery);
            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return _mapper.Map<IEnumerable<TerritoryDto>>(territories)
                .ToPagedResponse(paginationQuery, totalItems, next, previous);
        }

        public async Task<Response<TerritoryDto>> FindByIdAsync(string id)
        {
            var territory = await _unitOfWork.Territories.FindByIdAsync(id);

            return _mapper.Map<TerritoryDto>(territory).ToResponse();
        }

        public async Task<Response<TerritoryDto>> CreateAsync(TerritoryDto territoryDto)
        {
            var territory = _mapper.Map<Territory>(territoryDto);

            await _unitOfWork.Territories.AddAsync(territory);
            await _unitOfWork.CompleteAsync();

            territoryDto.TerritoryId = territory.TerritoryId;

            return territoryDto.ToResponse();
        }

        public async Task<Response<TerritoryDto>> UpdateAsync(TerritoryDto territoryDto)
        {
            var territoryInDb = await _unitOfWork.Territories.FindByIdAsync(territoryDto.TerritoryId);
            _mapper.Map(territoryDto, territoryInDb);

            await _unitOfWork.CompleteAsync();

            return territoryDto.ToResponse();
        }

        public async Task<Response<IEnumerable<TerritoryDto>>> DeleteAsync(string[] ids)
        {
            var territories = (await _unitOfWork.Territories.GetAsync(predicate: t => ids.Contains(t.TerritoryId))).items;

            _unitOfWork.Territories.Remove(territories);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<TerritoryDto>>(territories).ToResponse();
        }

        public async Task<bool> IsExists(string id)
        {
            return await _unitOfWork.Territories.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(string[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Territories.GetAsync(predicate: t => ids.Contains(t.TerritoryId))).items.Count() == ids.Length;
        }
    }
}

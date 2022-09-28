using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RegionDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var regions = await _unitOfWork.Regions.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<RegionDto>>(regions);
        }

        public async Task<RegionDto>? GetAsync(int id)
        {
            var region = await _unitOfWork.Regions.GetAsync(id);

            return _mapper.Map<RegionDto>(region);
        }

        public async Task<int> CreateAsync(RegionDto regionDto)
        {
            var region = _mapper.Map<Region>(regionDto);

            await _unitOfWork.Regions.AddAsync(region);
            await _unitOfWork.CompleteAsync();

            return region.RegionId;
        }

        public async Task UpdateAsync(RegionDto regionDto)
        {
            var regionInDb = await _unitOfWork.Regions.GetAsync(regionDto.RegionId);

            _mapper.Map(regionDto, regionInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<RegionDto> DeleteAsync(int id)
        {
            var region = await _unitOfWork.Regions.GetAsync(id);

            _unitOfWork.Regions.Remove(region);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<RegionDto>(region);
        }

        public async Task<IEnumerable<RegionDto>> DeleteRangeAsync(int[] ids)
        {
            var regions = await _unitOfWork.Regions.FindAllAsync(r => ids.Contains(r.RegionId));

            _unitOfWork.Regions.RemoveRange(regions);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<RegionDto>>(regions);
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Regions.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Regions.FindAllAsync(r => ids.Contains(r.RegionId))).Count() == ids.Length;
        }
    }
}

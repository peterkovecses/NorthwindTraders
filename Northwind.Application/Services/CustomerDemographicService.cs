using AutoMapper;
using Northwind.Application.Common.Extensions;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Common.Responses;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class CustomerDemographicService : ICustomerDemographicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public CustomerDemographicService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<Response<IEnumerable<CustomerDemographicDto>>> GetAllAsync()
        {
            var customerDemographics = await _unitOfWork.CustomerDemographics.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDemographicDto>>(customerDemographics).ToResponse();
        }

        public async Task<PagedResponse<CustomerDemographicDto>> GetAllAsync(PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var (totalItems, customerDemographics) = await _unitOfWork.CustomerDemographics.GetAllAsync(paginationFilter);
            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return _mapper.Map<IEnumerable<CustomerDemographicDto>>(customerDemographics)
                .ToPagedResponse(paginationQuery, totalItems, next, previous);
        }

        public async Task<Response<CustomerDemographicDto>> GetAsync(string id)
        {
            var customerDemographic = await _unitOfWork.CustomerDemographics.GetAsync(id);

            return _mapper.Map<CustomerDemographicDto>(customerDemographic).ToResponse();
        }

        public async Task<Response<CustomerDemographicDto>> CreateAsync(CustomerDemographicDto customerDemographicDto)
        {
            var customerDemographic = _mapper.Map<CustomerDemographic>(customerDemographicDto);

            await _unitOfWork.CustomerDemographics.AddAsync(customerDemographic);
            await _unitOfWork.CompleteAsync();

            customerDemographicDto.CustomerTypeId = customerDemographic.CustomerTypeId;

            return customerDemographicDto.ToResponse();
        }

        public async Task<Response<CustomerDemographicDto>> UpdateAsync(CustomerDemographicDto customerDemographicDto)
        {
            var customerDemographicInDb = await _unitOfWork.CustomerDemographics.GetAsync(customerDemographicDto.CustomerTypeId);

            _mapper.Map(customerDemographicDto, customerDemographicInDb);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CustomerDemographicDto>(customerDemographicInDb).ToResponse();
        }

        public async Task<Response<CustomerDemographicDto>> DeleteAsync(string id)
        {
            var customerDemographic = await _unitOfWork.CustomerDemographics.GetAsync(id);

            _unitOfWork.CustomerDemographics.Remove(customerDemographic);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CustomerDemographicDto>(customerDemographic).ToResponse();
        }

        public async Task<Response<IEnumerable<CustomerDemographicDto>>> DeleteRangeAsync(string[] ids)
        {
            var customerDemographics = await _unitOfWork.CustomerDemographics.FindAllAsync(x => ids.Contains(x.CustomerTypeId));

            _unitOfWork.CustomerDemographics.RemoveRange(customerDemographics);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CustomerDemographicDto>>(customerDemographics).ToResponse();
        }

        public async Task<bool> IsExists(string id)
        {
            return await _unitOfWork.CustomerDemographics.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(string[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.CustomerDemographics.FindAllAsync(x => ids.Contains(x.CustomerTypeId))).Count() == ids.Length;
        }
    }
}

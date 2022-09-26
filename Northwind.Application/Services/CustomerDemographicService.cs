using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
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

        public CustomerDemographicService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDemographicDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var customerDemographics = await _unitOfWork.CustomerDemographics.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<CustomerDemographicDto>>(customerDemographics);
        }

        public async Task<CustomerDemographicDto>? GetAsync(string id)
        {
            var customerDemographic = await _unitOfWork.CustomerDemographics.GetAsync(id);

            return _mapper.Map<CustomerDemographicDto>(customerDemographic);
        }

        public async Task<string> CreateAsync(CustomerDemographicDto customerDemographicDto)
        {
            var customerDemographic = _mapper.Map<CustomerDemographic>(customerDemographicDto);

            await _unitOfWork.CustomerDemographics.AddAsync(customerDemographic);
            await _unitOfWork.CompleteAsync();

            return customerDemographic.CustomerTypeId;
        }

        public async Task UpdateAsync(CustomerDemographicDto customerDemographicDto)
        {
            var customerDemographicInDb = await _unitOfWork.CustomerDemographics.GetAsync(customerDemographicDto.CustomerTypeId);

            _mapper.Map(customerDemographicDto, customerDemographicInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<CustomerDemographicDto> DeleteAsync(string id)
        {
            var customerDemographic = await _unitOfWork.CustomerDemographics.GetAsync(id);

            _unitOfWork.CustomerDemographics.Remove(customerDemographic);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CustomerDemographicDto>(customerDemographic);
        }

        public async Task<IEnumerable<CustomerDemographicDto>> DeleteRangeAsync(string[] ids)
        {
            var customerDemographics = await _unitOfWork.CustomerDemographics.FindAllAsync(x => ids.Contains(x.CustomerTypeId));

            _unitOfWork.CustomerDemographics.RemoveRange(customerDemographics);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CustomerDemographicDto>>(customerDemographics);
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

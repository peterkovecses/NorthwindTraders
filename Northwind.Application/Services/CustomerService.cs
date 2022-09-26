using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var customers = await _unitOfWork.Customers.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto>? GetAsync(string id)
        {
            var customer = await _unitOfWork.Customers.GetAsync(id);

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<string> CreateAsync(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.CompleteAsync();

            return customer.CustomerId;
        }

        public async Task UpdateAsync(CustomerDto customerDto)
        {
            var customerInDb = await _unitOfWork.Customers.GetAsync(customerDto.CustomerId);

            _mapper.Map(customerDto, customerInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<CustomerDto> DeleteAsync(string id)
        {
            var customer = await _unitOfWork.Customers.GetAsync(id);

            _unitOfWork.Customers.Remove(customer);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<IEnumerable<CustomerDto>> DeleteRangeAsync(string[] ids)
        {
            var customers = await _unitOfWork.Customers.FindAllAsync(c => ids.Contains(c.CustomerId));

            _unitOfWork.Customers.RemoveRange(customers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<bool> IsExists(string id)
        {
            return await _unitOfWork.Customers.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(string[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Customers.FindAllAsync(c => ids.Contains(c.CustomerId))).Count() == ids.Length;
        }
    }
}

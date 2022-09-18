using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;

namespace Northwind.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAsync()
        {
            var employees = await _unitOfWork.Employees.GetAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public Task<EmployeeDto>? GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateAsync(EmployeeDto employee)
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeDto> DeleteAsync(EmployeeDto employee)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EmployeeDto>> DeleteRangeAsync(IEnumerable<EmployeeDto> employees)
        {
            throw new NotImplementedException();
        }
    }
}

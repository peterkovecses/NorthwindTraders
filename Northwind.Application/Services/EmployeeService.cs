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
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<Response<IEnumerable<EmployeeDto>>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees).ToResponse();
        }

        public async Task<PagedResponse<EmployeeDto>> GetAllAsync(PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var (totalItems, employees) = await _unitOfWork.Employees.GetAllAsync(paginationFilter);
            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees)
                .ToPagedResponse(paginationQuery, totalItems, next, previous);
        }

        public async Task<Response<EmployeeDto>> GetAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetAsync(id);

            return _mapper.Map<EmployeeDto>(employee).ToResponse();
        }

        public async Task<Response<EmployeeDto>> CreateAsync(EmployeeDto employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CompleteAsync();

            employeeDto.EmployeeId = employee.EmployeeId;

            return employeeDto.ToResponse();
        }

        public async Task<Response<EmployeeDto>> UpdateAsync(EmployeeDto employeeDto)
        {
            var employeeInDb = await _unitOfWork.Employees.GetAsync(employeeDto.EmployeeId);

            _mapper.Map(employeeDto, employeeInDb);
            await _unitOfWork.CompleteAsync();

            return employeeDto.ToResponse();
        }

        public async Task<Response<EmployeeDto>> DeleteAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetAsync(id);

            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<EmployeeDto>(employee).ToResponse();
        }

        public async Task<Response<IEnumerable<EmployeeDto>>> DeleteRangeAsync(int[] ids)
        {
            var employees = await _unitOfWork.Employees.FindAllAsync(e => ids.Contains(e.EmployeeId));

            _unitOfWork.Employees.RemoveRange(employees);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Employees.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Employees.FindAllAsync(e => ids.Contains(e.EmployeeId))).Count() == ids.Length;
        }
    }
}

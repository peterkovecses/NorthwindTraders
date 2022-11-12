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
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<EmployeeDto>> GetAsync(QueryParameters<EmployeeFilter> queryParameters, CancellationToken token)
        {            
            (int totalEmployees, IEnumerable<Employee> employees) = await _unitOfWork.Employees.GetAsync(queryParameters.Pagination, queryParameters.Sorting, queryParameters.Filter.GetPredicate(), token);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees)
                .ToPagedResponse(queryParameters.Pagination, totalEmployees);
        }

        public async Task<Response<EmployeeDto>> FindByIdAsync(int id, CancellationToken token)
        {
            var employee = await _unitOfWork.Employees.FindByIdAsync(id, token);

            return _mapper.Map<EmployeeDto>(employee).ToResponse();
        }

        public async Task<Response<EmployeeDto>> CreateAsync(EmployeeDto employeeDto, CancellationToken token)
        {
            var employee = _mapper.Map<Employee>(employeeDto);

            await _unitOfWork.Employees.AddAsync(employee, token);
            await _unitOfWork.CompleteAsync();

            employeeDto.EmployeeId = employee.EmployeeId;

            return employeeDto.ToResponse();
        }

        public async Task<Response<EmployeeDto>> UpdateAsync(EmployeeDto employeeDto, CancellationToken token)
        {
            var employeeInDb = 
                await _unitOfWork.Employees.FindByIdAsync(employeeDto.EmployeeId, token) ?? throw new ItemNotFoundException<int>(employeeDto.EmployeeId);
            _mapper.Map(employeeDto, employeeInDb);
            await _unitOfWork.CompleteAsync();

            return employeeDto.ToResponse();
        }

        public async Task<Response<IEnumerable<EmployeeDto>>> DeleteAsync(int[] ids, CancellationToken token)
        {
            var employeesToRemove = (await _unitOfWork.Employees.GetAsync(Pagination.NoPagination(), Sorting.NoSorting(), e => ids.Contains(e.EmployeeId), token)).items;

            foreach (var employee in employeesToRemove)
            {
                _unitOfWork.Employees.Remove(employee);
            }
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(employeesToRemove).ToResponse();
        }       
    }
}

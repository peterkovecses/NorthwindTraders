using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Queries;
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

        public async Task<PagedResponse<EmployeeDto>> GetAsync(QueryParameters queryParameters)
        {
            var (totalItems, employees) = await _unitOfWork.Employees.GetAsync(queryParameters.Pagination, queryParameters.Sorting);
            queryParameters.SetPaginationIfNull(totalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees)
                .ToPagedResponse(queryParameters.Pagination, totalItems, next, previous);
        }

        public async Task<Response<EmployeeDto>> FindByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.FindByIdAsync(id);

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
            var employeeInDb = await _unitOfWork.Employees.FindByIdAsync(employeeDto.EmployeeId);

            _mapper.Map(employeeDto, employeeInDb);
            await _unitOfWork.CompleteAsync();

            return employeeDto.ToResponse();
        }

        public async Task<Response<IEnumerable<EmployeeDto>>> DeleteAsync(int[] ids)
        {
            var employees = (await _unitOfWork.Employees.GetAsync(predicate: e => ids.Contains(e.EmployeeId))).items;

            _unitOfWork.Employees.Remove(employees);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Employees.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Employees.GetAsync(predicate: e => ids.Contains(e.EmployeeId))).items.Count() == ids.Length;
        }
    }
}

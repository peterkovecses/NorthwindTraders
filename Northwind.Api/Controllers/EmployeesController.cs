using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Extensions;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;

namespace Northwind.Api.Controllers
{
    [Route("employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IPaginatedUriService _uriService;

        public EmployeesController(IEmployeeService employeeService, IPaginatedUriService uriService)
        {
            _employeeService = employeeService;
            _uriService = uriService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] PaginationQuery paginationQuery)
        {
            var employees = await _employeeService.GetAllAsync(paginationQuery);

            if (paginationQuery == null || paginationQuery.PageNumber < 1 || paginationQuery.PageSize < 1)
            {
                return Ok(employees.ToPagedResponse());
            }

            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return Ok(employees.ToPagedResponse().AddPagination(paginationQuery, next, previous));
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeService.GetAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee.ToResponse());
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeDto employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employee.EmployeeId = await _employeeService.CreateAsync(employee);

            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee.ToResponse());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.EmployeeId)
            {
                return BadRequest(ModelState);
            }

            if (!_employeeService.IsExists(id).Result)
            {
                return NotFound();
            }

            try
            {
                await _employeeService.UpdateAsync(employee);

                return Ok(employee.ToResponse());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _employeeService.IsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteEmployees([FromQuery] int[] ids)
        {
            if (!await _employeeService.AreExists(ids))
            {
                return NotFound();
            }

            var employees = await _employeeService.DeleteRangeAsync(ids);

            return Ok(employees.ToResponse());
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Extensions;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Api.Controllers
{
    [Route("employees")]
    [ApiController]
    public class EmployeesController : ApiControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(
            IEmployeeService employeeService, 
            ILogger<EmployeesController> logger) 
            : base(logger)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] QueryParameters<EmployeeFilter, Employee> queryParameters, CancellationToken token)
        {
            var response = (await _employeeService.GetAsync(queryParameters, token)).SetNavigation(BaseUri);

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> GetEmployee(int id, CancellationToken token)
        {
            var response = await _employeeService.FindByIdAsync(id, token);

            return GetResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeDto employee, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.CreateAsync(employee, token);

            return CreatedAtAction("GetEmployee", new { id = response.Data.EmployeeId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto employee, CancellationToken token)
        {
            if (id != employee.EmployeeId)
            {
                ModelState.AddModelError("id", IdsNotMatchMessage);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.UpdateAsync(employee, token);

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteEmployees([FromQuery] int[] ids, CancellationToken token)
        {
            var response = await _employeeService.DeleteAsync(ids, token);

            return Ok(response);
        }
    }
}

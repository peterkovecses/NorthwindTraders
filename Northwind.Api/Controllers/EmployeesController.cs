using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;

namespace Northwind.Api.Controllers
{
    [Route("employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] QueryParameters<EmployeeFilter> queryParameters, CancellationToken token)
        {
            var response = await _employeeService.GetAsync(queryParameters, token);

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> GetEmployee(int id, CancellationToken token)
        {                      
            var response = await _employeeService.FindByIdAsync(id, token);

            if (response.Data == null)
            {
                return NotFound();
            }

            return Ok(response);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.EmployeeId)
            {
                return BadRequest(ModelState);
            }

            if (!await _employeeService.IsExists(id, token))
            {
                return NotFound();
            }

            var response = await _employeeService.UpdateAsync(employee, token);

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteEmployees([FromQuery] int[] ids, CancellationToken token)
        {
            if (!await _employeeService.AreExists(ids, token))
            {
                return NotFound();
            }

            var response = await _employeeService.DeleteAsync(ids, token);

            return Ok(response);
        }
    }
}

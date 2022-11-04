using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Extensions;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;

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
        public async Task<IActionResult> GetEmployees([FromQuery] QueryParameters<EmployeeFilter> queryParameters, CancellationToken token)
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.EmployeeId)
            {
                return BadRequest(ModelState);
            }

            Response<EmployeeDto> response;
            try
            {
                response = await _employeeService.UpdateAsync(employee, token);
            }
            catch (ItemNotFoundException ex)
            {
                _logger.LogError(ex);
                return NotFound();
            }

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

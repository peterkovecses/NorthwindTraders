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
    [Route("customers")]
    [ApiController]
    public class CustomersController : ApiControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(
            ICustomerService customerService, 
            ILogger<CustomersController> logger) 
            : base(logger)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] QueryParameters<CustomerFilter> queryParameters, CancellationToken token)
        {
            var response = (await _customerService.GetAsync(queryParameters, token)).SetNavigation(BaseUri); ;

            return Ok();
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> GetCustomer(string id, CancellationToken token)
        {
            var response = await _customerService.FindByIdAsync(id, token);

            return GetResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerDto customer, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _customerService.CreateAsync(customer, token);

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(string id, CustomerDto customer, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerId)
            {
                return BadRequest(ModelState);
            }

            Response<CustomerDto> response;
            try
            {
                response = await _customerService.UpdateAsync(customer, token);
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
        public async Task<IActionResult> DeleteCustomers([FromQuery] string[] ids, CancellationToken token)
        {
            var response = await _customerService.DeleteAsync(ids, token);

            return Ok(response);
        }
    }
}

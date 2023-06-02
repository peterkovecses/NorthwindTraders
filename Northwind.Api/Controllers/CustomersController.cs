using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Extensions;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;
using Northwind.Infrastructure.Claims;

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
        [Authorize(Policy = ClaimPolicies.CustomerViewer)]
        public async Task<IActionResult> GetCustomers([FromQuery] QueryParameters<CustomerFilter, Customer> queryParameters, CancellationToken token)
        {
            var response = (await _customerService.GetAsync(queryParameters, token)).SetNavigation(BaseUri); ;

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        [Authorize(Policy = ClaimPolicies.CustomerViewer)]
        public async Task<IActionResult> GetCustomer(string id, CancellationToken token)
        {
            var response = await _customerService.FindByIdAsync(id, token);

            return GetResult(response);
        }

        [HttpPost]
        [Authorize(Policy = ClaimPolicies.CustomerAdministrator)]
        public async Task<IActionResult> CreateCustomer(CustomerDto customer, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCustomer = (await _customerService.FindByIdAsync(customer.CustomerId, token)).Data;

            if (existingCustomer != null)
            {
                return Conflict("A Customer with the same CustomerId already exists.");
            }

            var response = await _customerService.CreateAsync(customer, token);

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = ClaimPolicies.CustomerAdministrator)]
        public async Task<IActionResult> UpdateCustomer(string id, CustomerDto customer, CancellationToken token)
        {
            if (id != customer.CustomerId)
            {
                ModelState.AddModelError("id", IdsNotMatchMessage);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }            

            var response = await _customerService.UpdateAsync(customer, token);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = ClaimPolicies.CustomerAdministrator)]
        public async Task<IActionResult> DeleteCustomers(string id, CancellationToken token)
        {
            await _customerService.DeleteAsync(id, token);

            return Ok();
        }
    }
}

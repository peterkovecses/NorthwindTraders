using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Extensions;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;

namespace Northwind.Api.Controllers
{
    [Route("customerdemographics")]
    [ApiController]
    public class CustomerDemographicsController : ApiControllerBase
    {
        private readonly ICustomerDemographicService _customerDemographicService;

        public CustomerDemographicsController(
            ICustomerDemographicService customerDemographicService, 
            ILogger<CustomerDemographicsController> logger) 
            : base(logger)
        {
            _customerDemographicService = customerDemographicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerDemographic(
            [FromQuery] QueryParameters<CustomerDemographicFilter> queryParameters, 
            CancellationToken token)
        {
            var response = (await _customerDemographicService.GetAsync(queryParameters, token)).SetNavigation(BaseUri); ;

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetCustomerDemographic")]
        public async Task<IActionResult> GetCustomerDemographic(string id, CancellationToken token)
        {
            var response = await _customerDemographicService.FindByIdAsync(id, token);

            return GetResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerDemographic(CustomerDemographicDto customerDemographicDto, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _customerDemographicService.CreateAsync(customerDemographicDto, token);

            return CreatedAtAction("GetCustomerDemographic", new { id = response.Data.CustomerTypeId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerDemographic(
            string id, 
            CustomerDemographicDto customerDemographicDto, 
            CancellationToken token)
        {
            if (id != customerDemographicDto.CustomerTypeId)
            {
                ModelState.AddModelError("id", IdsNotMatchMessage);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _customerDemographicService.UpdateAsync(customerDemographicDto, token);

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCustomerDemographics([FromQuery] string[] ids, CancellationToken token)
        {
            var response = await _customerDemographicService.DeleteAsync(ids, token);

            return Ok(response);
        }
    }
}

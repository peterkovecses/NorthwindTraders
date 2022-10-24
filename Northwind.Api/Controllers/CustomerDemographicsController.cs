using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;

namespace Northwind.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDemographicsController : ControllerBase
    {
        private readonly ICustomerDemographicService _customerDemographicService;
        private readonly IPaginatedUriService _uriService;
        private readonly ILogger<CustomerDemographicsController> _logger;

        public CustomerDemographicsController(ICustomerDemographicService customerDemographicService, IPaginatedUriService uriService, ILogger<CustomerDemographicsController> logger)
        {
            _customerDemographicService = customerDemographicService;
            _uriService = uriService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerDemographic(
            [FromQuery] QueryParameters<CustomerDemographicFilter> queryParameters, 
            CancellationToken token)
        {
            var response = await _customerDemographicService.GetAsync(queryParameters, token);
            (response.NextPage, response.PreviousPage) = _uriService.GetNavigations(queryParameters.Pagination);

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetCustomerDemographic")]
        public async Task<IActionResult> GetCustomerDemographic(string id, CancellationToken token)
        {
            var response = await _customerDemographicService.FindByIdAsync(id, token);

            if (response.HasData)
            {
                return Ok(response);
            }

            return NotFound();
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerDemographicDto.CustomerTypeId)
            {
                return BadRequest(ModelState);
            }

            Response<CustomerDemographicDto> response;
            try
            {
                response = await _customerDemographicService.UpdateAsync(customerDemographicDto, token);
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
        public async Task<IActionResult> DeleteCustomerDemographics([FromQuery] string[] ids, CancellationToken token)
        {
            var response = await _customerDemographicService.DeleteAsync(ids, token);

            return Ok(response);
        }
    }
}

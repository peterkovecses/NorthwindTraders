using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Models;

namespace Northwind.Api.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {        
        protected readonly ILogger<ApiControllerBase> _logger;

        protected const string IdsNotMatchMessage = "The specified id does not match the id of the object to be modified.";


        protected string BaseUri => string.Concat($"{Request.Scheme}://{Request.Host.ToUriComponent()}{Request.Path}", "/");

        public ApiControllerBase(ILogger<ApiControllerBase> logger)
        {
            _logger = logger;
        }

        protected IActionResult GetResult<T>(Response<T> response)
        {
            if (response.HasData)
            {
                return Ok(response);
            }

            return NotFound();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Models;

namespace Northwind.Api.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected readonly ILogger<ApiControllerBase> _logger;
        private string? _baseUri;

        protected string? BaseUri
        {
            get
            {
                _baseUri ??= GetAbsoluteUri();
                return _baseUri;
            }
        }

        public ApiControllerBase(ILogger<ApiControllerBase> logger)
        {
            _logger = logger;
        }

        private string GetAbsoluteUri()
        {
            return string.Concat($"{Request.Scheme}://{Request.Host.ToUriComponent()}{Request.Path}", "/");
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

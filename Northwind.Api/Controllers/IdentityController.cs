using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Models;
using Northwind.Application.Interfaces;
using Northwind.Application.Models;

namespace Northwind.Api.Controllers
{
    [Route("identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage))
                });
            }

            var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);

            if(!authResponse.Success)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
#pragma warning restore CS8601 // Possible null reference assignment.
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            return Ok(new AuthSuccesResponse { Token =  authResponse.Token});
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
#pragma warning restore CS8601 // Possible null reference assignment.
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            return Ok(new AuthSuccesResponse { Token = authResponse.Token });
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
}

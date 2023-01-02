using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, Tester")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage))
                });
            }

            var response = 
                await _identityService.RegisterAsync(request.Email, request.Password, request.ClaimTypes, request.Roles);

            if(!response.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = response.Errors! });
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors! });
            }

            return Ok(new AuthSuccesResponse 
            {
                Token = authResponse.Token!,
                RefreshToken = authResponse.RefreshToken!
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] Models.RefreshTokenRequest request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors! });
            }

            return Ok(new AuthSuccesResponse
            {
                Token = authResponse.Token!,
                RefreshToken = authResponse.RefreshToken!
            });
        }
    }
}

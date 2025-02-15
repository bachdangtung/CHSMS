using CHSMS.API.DTOs;
using CHSMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CHSMS.API.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Login (Returns JWT Token)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var token = await _authService.AuthenticateAsync(model.Email, model.Password);
            if (token == null)
                return Unauthorized("Invalid credentials.");

            return Ok(new { Token = token });
        }
    }
}

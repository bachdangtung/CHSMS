using CHSMS.API.DTOs.User;
using CHSMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            var result = await _authService.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
            if (!result)
                return BadRequest("Incorrect old password.");

            return Ok("Password changed successfully.");
        }

        // Request Password Reset
        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            bool result = await _authService.RequestResetPasswordAsync(model.Email);
            if (!result)
                return BadRequest("User not found.");

            return Ok("Password reset link has been sent to your email.");
        }

        // Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            bool result = await _authService.ResetPasswordAsync(model.Token, model.NewPassword);
            if (!result)
                return BadRequest("Invalid or expired token.");

            return Ok("Password reset successfully.");
        }
    }
}

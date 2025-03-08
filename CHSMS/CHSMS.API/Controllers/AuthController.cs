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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await _authService.AuthenticateAsync(model.UserName, model.Password);
            if (token == null)
                return Unauthorized("Sai tài khoản hoặc mật khẩu.");

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            var result = await _authService.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
            if (!result)
                return BadRequest("Sai mật khẩu cũ.");

            return Ok("Đã thay đổi mật khẩu.");
        }

        // Request Password Reset
        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool result = await _authService.RequestResetPasswordAsync(model.Email);
            if (!result)
                return BadRequest("Không tìm thấy Email.");

            return Ok("Đã gửi Email thành công.");
        }

        // Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result)
                return BadRequest("Đường đẫn không hợp lệ hoặc hết hạn");

            return Ok("Đặt lại mật khẩu thành công.");
        }

        //Add user
        [Authorize(Roles = "Trưởng trạm")]
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _authService.CreateUserAsync(createUserDto);
                return Ok(new { message = "Tạo tài khoản thành công", user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

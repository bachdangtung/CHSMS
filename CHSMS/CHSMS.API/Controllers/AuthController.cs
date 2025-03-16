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
        [HttpPost("/api/Authen/Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await _authService.AuthenticateAsync(model.UserName, model.Password);
            if (token == "inactive")
            {
                return Unauthorized("Tài khoản không tồn tại hoặc đã bị vô hiệu hóa.");
            }
            if (token == null)
                return Unauthorized("Sai tài khoản hoặc mật khẩu.");

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpPost("/api/Authen/ChangePassword")]
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
        [HttpPost("/api/Authen/RequestResetPassword")]
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
        [HttpPost("/api/Authen/ResetPassword")]
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
        [HttpPost("/api/User/AddUser")]
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

        //Deactive user
        [Authorize(Roles = "Trưởng trạm")]
        [HttpPost("/api/User/ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            try
            {
                var result = _authService.ChangeStatusAsync(id);
                if (result.Result)
                {
                    return Ok("Đã đổi trạng thái");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Trưởng trạm")]
        [HttpGet("/api/User/List")]
        public async Task<IActionResult> GetUserList()
        {
            try
            {
                var users = await _authService.GetUserListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("/api/User/Profile")]
        public async Task<ActionResult<UserListDto>> UserProfile()
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            var user = await _authService.GetUserProfileAsync(userId);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPut("/api/User/EditProfile")]
        public async Task<IActionResult> EditUserProfile([FromBody] EditUserProfileDto editUserProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            var result = await _authService.EditUserProfileAsync(userId, editUserProfileDto);
            if (!result)
            {
                return BadRequest("Cập nhật hồ sơ thất bại.");
            }

            return Ok("Hồ sơ đã được cập nhật thành công.");
        }
    }
}

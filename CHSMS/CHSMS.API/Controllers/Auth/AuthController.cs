using CHSMS.API.DTOs.User;
using CHSMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;

namespace CHSMS.API.Controllers.Auth
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        public AuthController(IAuthService authService, IDistributedCache cache, IConfiguration configuration)
        {
            _authService = authService;
            _cache = cache;
            _configuration = configuration;
        }

        // Login (Returns JWT Token)
        /*        [HttpPost("/api/Authen/Login")]
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
                }*/

        [HttpPost("/api/Authen/Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tokenPair = await _authService.AuthenticateAsync(model.UserName, model.Password);
            if (tokenPair?.AccessToken == "inactive")
            {
                return Unauthorized("Tài khoản không tồn tại hoặc đã bị vô hiệu hóa.");
            }
            if (tokenPair == null)
                return Unauthorized("Sai tài khoản hoặc mật khẩu.");

            return Ok(tokenPair);
        }

        [HttpPost("/api/Authen/RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto request)
        {
            var tokenPair = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
            if (tokenPair == null)
            {
                return BadRequest("Invalid token or refresh token");
            }

            return Ok(tokenPair);
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
        [HttpPost("/api/User/ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            try
            {
                var result = await _authService.ChangeStatusAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã đổi trạng thái" });
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
        [HttpGet("/api/User/Profile")]
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

        /*        [Authorize]
                [HttpPost("/api/Authen/Logout")]
                public async Task<IActionResult> Logout()
                {
                    var userId = User.FindFirst("Id")?.Value;
                    var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jti))
                    {
                        return BadRequest("Invalid token claims.");
                    }

                    // Read expiry time from appsettings.json (200 minutes)
                    var expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

                    var cacheKey = $"blacklist:{jti}";
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiryInMinutes)
                    };

                    await _cache.SetStringAsync(cacheKey, userId, options);

                    return Ok("Logged out successfully.");
                }*/

        [Authorize]
        [HttpPost("/api/Authen/Logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(jti))
            {
                return BadRequest("Invalid token claims.");
            }

            // Revoke refresh token
            await _authService.RevokeRefreshToken(userId);

            // Blacklist current token
            var expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);
            var cacheKey = $"blacklist:{jti}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiryInMinutes)
            };

            await _cache.SetStringAsync(cacheKey, userId.ToString(), options);

            return Ok("Logged out successfully.");
        }

        [HttpGet("Blacklist/{jti}")]
        public async Task<IActionResult> IsTokenBlacklisted(string jti)
        {
            var cacheKey = $"blacklist:{jti}";
            var value = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(value))
                return NotFound($"Token is not blacklisted.\n{value}");

            return Ok("Token is blacklisted.");
        }

        [Authorize(Roles = "Trưởng trạm")]
        [HttpGet("/api/User/GetAll")]
        public async Task<IActionResult> GetUserList(
            [FromQuery] string? search,
            [FromQuery] string? gender,
            [FromQuery] bool? status,
            [FromQuery] int? roleId)
        {
            try
            {
                var users = await _authService.GetUserListAsync(search, gender, status, roleId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

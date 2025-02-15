using CHSMS.API.DTOs;
using CHSMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            using (SEP_TestContext context = new SEP_TestContext())
            {
                // Kiểm tra xem model có hợp lệ không
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Tìm người dùng trong cơ sở dữ liệu bằng email
                var user = await context.Users.Include(r => r.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                // Kiểm tra xem người dùng có tồn tại không
                if (user == null)
                {
                    return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác." });
                }

                // Kiểm tra mật khẩu (trong thực tế, mật khẩu nên được băm và so sánh)
                if (user.Password != model.Password)
                {
                    return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác." });
                }

                // Trả về thông tin người dùng và token
                return Ok(new
                {
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.RoleName,
                });
            }
        }
    }
}

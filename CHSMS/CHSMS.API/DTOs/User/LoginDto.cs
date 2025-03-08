using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Hãy nhập tên người dùng!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Hãy nhập mật khẩu!")]
        public string Password { get; set; }
    }
}

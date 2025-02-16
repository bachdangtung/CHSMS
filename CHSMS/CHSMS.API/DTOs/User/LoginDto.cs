using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Hãy nhập Email!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hãy nhập mật khẩu!")]
        public string Password { get; set; }
    }
}

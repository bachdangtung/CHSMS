using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.User
{
    public class ResetPasswordDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required(ErrorMessage = "Hãy nhập mật khẩu mới")]
        [RegularExpression(
"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]{8,32}$",
ErrorMessage = "Mật khẩu phải dài từ 8 đến 32 kí tự, chứa ít nhất một số, chữ in hoa và kí tự đặc biệt")]

        public string NewPassword { get; set; }
    }
}

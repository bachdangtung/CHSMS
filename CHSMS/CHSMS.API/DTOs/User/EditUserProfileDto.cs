using CHSMS.API.Configurations;
using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.User
{
    public class EditUserProfileDto
    {
        [Required(ErrorMessage = "Hãy nhập họ và tên")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Họ và tên phải từ 3 đến 50 kí tự")]
        public string? Fullname { get; set; }
        public string? Gender { get; set; }
        [MinimumAge(18, ErrorMessage = "Người dùng phải trên 18")]
        public DateTime? Dob { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email!")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
    ErrorMessage = "Hãy nhập Email hợp lệ (ví dụ: user@example.com)")]
        public string? Email { get; set; }
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 chữ số")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}

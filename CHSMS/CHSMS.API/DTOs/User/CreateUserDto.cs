using CHSMS.API.Configurations;
using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.User
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Hãy nhập tên người dùng")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên người dùng phải từ 3 đến 50 kí tự")]
        public string? UserName { get; set; }
        public string? Fullname { get; set; }
        public string? Gender { get; set; }
        [MinimumAge(18, ErrorMessage = "Người dùng phải trên 18")]
        public DateTime? Dob { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email!")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
    ErrorMessage = "Hãy nhập Email hợp lệ (ví dụ: user@example.com)")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Hãy nhập vai trò")]
        public int? RoleId { get; set; }
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}

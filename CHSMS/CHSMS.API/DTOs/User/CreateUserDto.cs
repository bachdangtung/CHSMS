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
        public DateTime? Dob { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email!")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Hãy nhập mật khẩu")]
        [RegularExpression(
"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]{8,32}$",
ErrorMessage = "Mật khẩu phải dài từ 8 đến 32 kí tự, chứa ít nhất một số, chữ in hoa và kí tự đặc biệt")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Hãy nhập vai trò")]
        public int? RoleId { get; set; }
        public int? DepartmentId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}

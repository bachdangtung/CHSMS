using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.User
{
    public class EditUserProfileDto
    {
        [Required(ErrorMessage = "Hãy nhập họ và tên")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Họ và tên phải từ 3 đến 50 kí tự")]
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email!")]
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.User
{
    public class ResetPasswordRequestDto
    {
        [Required(ErrorMessage = "Hãy nhập Email!")]
        [EmailAddress]
        public string Email { get; set; }
    }
}

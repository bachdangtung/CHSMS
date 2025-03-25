namespace CHSMS.API.DTOs.User
{
    public class UserListDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Department { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool? Status { get; set; }

    }
}

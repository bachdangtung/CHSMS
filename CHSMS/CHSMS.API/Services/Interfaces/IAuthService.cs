using CHSMS.API.DTOs.User;
using CHSMS.API.Models;

namespace CHSMS.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<bool> RequestResetPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<User> CreateUserAsync(CreateUserDto createUserDto);
    }
}

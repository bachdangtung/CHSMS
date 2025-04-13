using CHSMS.API.DTOs.User;
using CHSMS.API.Models;

namespace CHSMS.API.Services.Interfaces
{
    public interface IAuthService
    {
        /*        Task<string?> AuthenticateAsync(string email, string password);
        */
        Task<TokenPairDto?> AuthenticateAsync(string email, string password);
        Task<TokenPairDto?> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<bool> RevokeRefreshToken(int userId);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<bool> RequestResetPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<User> CreateUserAsync(CreateUserDto createUserDto);
        Task<bool> ChangeStatusAsync(int userId);
        Task<IEnumerable<UserListDto>> GetUserListAsync();
        Task<UserListDto> GetUserProfileAsync(int id);
        Task<bool> EditUserProfileAsync(int userId, EditUserProfileDto updatedUser);
        Task<IEnumerable<UserListDto>> GetUserListAsync(
           string? search, string? gender, bool? status, int? roleId);
    }
}

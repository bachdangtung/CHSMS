namespace CHSMS.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(string email, string password);
        /*        Task<bool> RequestResetPasswordAsync(string email);
                Task<bool> ResetPasswordAsync(string token, string newPassword);*/
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}

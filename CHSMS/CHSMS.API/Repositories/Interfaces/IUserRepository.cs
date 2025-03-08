using CHSMS.API.DTOs.User;
using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> GetByIdAsync(int id);
        void Update(User updatedUser);
        Task<User?> GetByResetTokenAsync(ResetPasswordDto resetPasswordDto);
        void Add(User newUser);
    }
}

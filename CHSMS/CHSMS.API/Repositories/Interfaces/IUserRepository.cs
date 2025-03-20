using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using System.Linq.Expressions;

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
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>>? filter = null);
    }
}

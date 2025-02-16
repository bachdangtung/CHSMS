using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        void Update(User UpdatedUser);
    }
}

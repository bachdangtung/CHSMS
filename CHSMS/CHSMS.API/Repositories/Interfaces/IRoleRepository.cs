using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<bool> RoleExistsAsync(int? roleId);
        Task<IEnumerable<Role?>> GetAllAsync();

    }
}

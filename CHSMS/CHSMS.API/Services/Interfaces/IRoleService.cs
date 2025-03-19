using CHSMS.API.DTOs.Role;

namespace CHSMS.API.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
    }
}

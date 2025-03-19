using CHSMS.API.DTOs.Department;

namespace CHSMS.API.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
    }
}

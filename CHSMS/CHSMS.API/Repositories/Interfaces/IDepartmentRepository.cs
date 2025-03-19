using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(int id);
        Task<bool> DepartmentExistsAsync(int? departmentId);
        Task<IEnumerable<Department?>> GetAllAsync();
    }
}

using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly SEP_TestContext _context;

        public DepartmentRepository(SEP_TestContext context)
        {
            _context = context;
        }
        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == id);
        }
        public async Task<bool> DepartmentExistsAsync(int? departmentId)
        {
            if (!departmentId.HasValue) return false;
            return await _context.Departments.AnyAsync(d => d.DepartmentId == departmentId.Value);
        }
    }
}

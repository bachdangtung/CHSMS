using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly SEP_TestContext _context;

        public RoleRepository(SEP_TestContext context)
        {
            _context = context;
        }
        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == id);
        }
        public async Task<bool> RoleExistsAsync(int? roleId)
        {
            if (!roleId.HasValue) return false;
            return await _context.Roles.AnyAsync(r => r.RoleId == roleId.Value);
        }
    }
}

using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SEP_TestContext _context;

        public UserRepository(SEP_TestContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public void Update(User UpdatedUser)
        {
            _context.Users.Update(UpdatedUser);
        }

        public async Task<User?> GetByResetTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == token);
        }

    }
}

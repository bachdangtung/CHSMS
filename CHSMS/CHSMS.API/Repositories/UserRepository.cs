using CHSMS.API.DTOs.User;
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

        public void Update(User updatedUser)
        {
            _context.Users.Update(updatedUser);
        }

        public void Add(User newUser)
        {
            _context.Users.Add(newUser);
        }

        public async Task<User?> GetByResetTokenAsync(ResetPasswordDto resetPasswordDto)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == resetPasswordDto.Token && u.UserId == resetPasswordDto.UserId);
        }

    }
}

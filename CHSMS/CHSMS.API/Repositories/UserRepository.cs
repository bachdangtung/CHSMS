using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByPhoneNumber(string phone)
        {
            return await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task UpdateAsync(User updatedUser)
        {
            _context.Users.Update(updatedUser);
            await _context.SaveChangesAsync(); // Save changes here
        }

        public async Task AddAsync(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(); // Save changes here
        }

        public async Task<User?> GetByResetTokenAsync(ResetPasswordDto resetPasswordDto)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == resetPasswordDto.Token && u.UserId == resetPasswordDto.UserId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Include(r => r.Role).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>>? filter = null)
        {
            IQueryable<User> query = _context.Users.Include(r => r.Role);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<int> CountActiveUser()
        {
            return await _context.Users.Where(r => r.Status == true).CountAsync();
        }
    }
}
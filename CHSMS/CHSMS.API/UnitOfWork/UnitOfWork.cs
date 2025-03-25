using CHSMS.API.Models;
using CHSMS.API.Repositories;
using CHSMS.API.Repositories.Interfaces;
namespace CHSMS.API.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SEP_TestContext _context;
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }

        public UnitOfWork(SEP_TestContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Roles = new RoleRepository(_context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}


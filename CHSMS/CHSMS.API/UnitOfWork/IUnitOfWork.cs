using CHSMS.API.Repositories.Interfaces;

namespace CHSMS.API.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IDepartmentRepository Departments { get; }
        IRoleRepository Roles { get; }
        Task<int> CommitAsync();
    }
}

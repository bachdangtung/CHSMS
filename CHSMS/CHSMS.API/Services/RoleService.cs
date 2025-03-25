using AutoMapper;
using CHSMS.API.DTOs.Role;
using CHSMS.API.Services.Interfaces;
using CHSMS.API.UnitOfWork;

namespace CHSMS.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roleList = await _unitOfWork.Roles.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roleList);
        }
    }
}

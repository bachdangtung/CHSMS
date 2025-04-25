using AutoMapper;
using CHSMS.API.DTOs.Role;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;

namespace CHSMS.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roleList = await _roleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roleList);
        }
    }
}
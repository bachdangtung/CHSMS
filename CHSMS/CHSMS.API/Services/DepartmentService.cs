using AutoMapper;
using CHSMS.API.DTOs.Department;
using CHSMS.API.Services.Interfaces;
using CHSMS.API.UnitOfWork;

namespace CHSMS.API.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var derpartmentList = await _unitOfWork.Departments.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartmentDto>>(derpartmentList);
        }
    }
}

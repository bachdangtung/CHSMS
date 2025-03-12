using AutoMapper;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;

namespace CHSMS.API.Configuration;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, LoginDto>()
            .ReverseMap();
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<User, UserListDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.RoleName))
            .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.DepartmentName));
    }
}

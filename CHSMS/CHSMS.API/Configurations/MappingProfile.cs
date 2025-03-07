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
    }
}

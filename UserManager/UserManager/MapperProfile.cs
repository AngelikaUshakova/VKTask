using AutoMapper;
using UserManager.Models;

namespace UserManager;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserGroup, UserGroupDto>().ReverseMap();
        CreateMap<UserState, UserStateDto>().ReverseMap();

        CreateMap<User, UserDto>();
    }
}
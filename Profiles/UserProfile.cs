using AutoMapper;
using ProjectLaborBackend.Dtos.UserDTOs;
using ProjectLaborBackend.Entities;

namespace ProjectLaborBackend.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<UserRegisterDTO, User>();
            CreateMap<UserLoginDTO, User>();
            CreateMap<UserPutDTO, User>();
            CreateMap<User, UserGetDTO>();
        }
    }
}

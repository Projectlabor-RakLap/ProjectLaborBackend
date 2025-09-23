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
            CreateMap<ForgotUserPutPasswordDTO, User>();
            CreateMap<UserPutPasswordDTO, User>().ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.NewPassword));
            CreateMap<User, UserGetDTO>();
        }
    }
}

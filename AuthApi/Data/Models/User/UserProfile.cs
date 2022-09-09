using AuthApi.Data.Models.User.UserViews;
using AutoMapper;

namespace AuthApi.Data.Models.User;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUser, User>();
        CreateMap<User, CreateUser>();

        CreateMap<GetUser, User>();
        CreateMap<User, GetUser>();

        CreateMap<UserIdentity, User>();
        CreateMap<User, UserIdentity>();

        CreateMap<UserPublic, User>();
        CreateMap<User, UserPublic>();
    }
}

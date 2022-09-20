using AutoMapper;
using Identity.Application.Dto.Users;
using Identity.Core.Aggregates;
using Identity.Core.Events.User;
using Identity.Core.ReadModels;

namespace Identity.Infrastructure.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<InitializedUserEvent, UserReadModel>();
        CreateMap<UserAggregateRoot, UserReadModel>();
        CreateMap<UserAggregateRoot, UserDto>();
    }
}
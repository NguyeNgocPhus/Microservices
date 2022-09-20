using Identity.Application.Dto.Users;
using Identity.Application.Dto.Users.Response;
using Identity.Application.GraphQL.ObjectType.Users;

namespace Identity.Application.GraphQL.Resolvers;

public interface IUserResolver
{
    Task<UserDto> TestRollBack(long id ,CancellationToken cancellationToken = default);
    Task<UpdateUserResponse> UpdateUserAsync(UpdateUserObjectType objectType, CancellationToken cancellationToken = default);
    Task<UserDto> DeleteUserAsync(long id, CancellationToken cancellationToken = default);
    Task<UserDto> GetUserByAdmin(long id, CancellationToken cancellationToken);
    Task<CreateUserResponse> CreateUserAsync(CreateUserObjectType objectType, CancellationToken cancellationToken = default);
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}
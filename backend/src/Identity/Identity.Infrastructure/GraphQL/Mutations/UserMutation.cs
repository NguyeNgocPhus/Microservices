using AppAny.HotChocolate.FluentValidation;
using Identity.Application.Dto.Users;
using Identity.Application.Dto.Users.Response;
using Identity.Application.GraphQL.Input.Users;
using Identity.Application.GraphQL.Resolvers;
using Identity.Infrastructure.Authorizations;
using Identity.Infrastructure.GraphqQL.Mutations;
using Identity.Infrastructure.Validators.Users;

namespace Identity.Infrastructure.GraphQL.Mutations;

public class UserMutation : ObjectTypeExtension<RootMutation>
{
    protected override void Configure(IObjectTypeDescriptor<RootMutation> descriptor)
    {
        descriptor.Field("createUserMutation")
            .Description("create user mutation")
            // .RequiredInternalRoles(new []{"Admin"})
            .Type<ObjectType<CreateUserResponse>>()
            .Argument("objectType", desc =>
            {
                desc.Type<CreateUserInput>();
                desc.UseFluentValidation(builder => builder.UseValidator(typeof(CreateUserValidator)));
            })
            .ResolveWith<IUserResolver>(resolve => resolve.CreateUserAsync(default,default));
        descriptor.Field("updateUserMutation")
            .Description("update user mutation")
            .Type<ObjectType<UpdateUserResponse>>()
            .Argument("objectType", desc =>
            {
                desc.Type<UpdateUserInput>();
                desc.UseFluentValidation(builder => builder.UseValidator(typeof(UpdateUserValidator)));
            })
            .ResolveWith<IUserResolver>(resolve => resolve.UpdateUserAsync(default,default));
        descriptor.Field("deleteUserMutation")
            .Description("delete user")
            .Argument("id", desc =>
            {
                desc.Description("User Id");
                desc.Type<LongType>();
            })
            .ResolveWith<IUserResolver>(resolve => resolve.DeleteUserAsync(default,default));
    }
}
using HotChocolate.Types;
using Identity.Application.GraphQL.ObjectType.Users;

namespace Identity.Application.GraphQL.Input.Users;

public class CreateUserInput: InputObjectType<CreateUserObjectType>
{
    protected override void Configure(IInputObjectTypeDescriptor<CreateUserObjectType> descriptor)
    {
        descriptor.Description("test");
        descriptor.Field(f => f.Name).Description("name is require");
        descriptor.Field(f => f.Password).Description("name is login");
        descriptor.Field(f => f.Email).Description("Email is require");
        base.Configure(descriptor);
    }
}
using HotChocolate.Types;
using Identity.Application.GraphQL.ObjectType.Users;

namespace Identity.Application.GraphQL.Input.Users;

public class UpdateUserInput:InputObjectType<UpdateUserObjectType>
{
    protected override void Configure(IInputObjectTypeDescriptor<UpdateUserObjectType> descriptor)
    {
        descriptor.Description("update ");
        descriptor.Field(f => f.Name).Description("name is require");
        descriptor.Field(f => f.Email).Description("Email is require");
        base.Configure(descriptor);
    } 
}
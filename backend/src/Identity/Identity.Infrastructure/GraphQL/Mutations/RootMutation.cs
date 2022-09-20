using Identity.Application.GraphQL.Mutations;

namespace Identity.Infrastructure.GraphqQL.Mutations;


public class RootMutation:IRootMutation
{
    
}
public class RootMutationType : ObjectType<RootMutation>
{
    protected override void Configure(IObjectTypeDescriptor<RootMutation> descriptor)
    {
        descriptor.Name("RootMutation");
        descriptor.Description("Root Mutation of the system");
    }
}
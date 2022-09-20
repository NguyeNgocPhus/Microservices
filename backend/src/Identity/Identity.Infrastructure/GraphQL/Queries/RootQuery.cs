using Identity.Application.GraphQL.Queries;

namespace Identity.Infrastructure.GraphqQL.Queries;

public class RootQuery:IRootQuery
{
    
}
public class RootQueryType : ObjectType<RootQuery>
{
    protected override void Configure(IObjectTypeDescriptor<RootQuery> descriptor)
    {
        descriptor.Name("RootQuery");
        descriptor.Description("Root Query of the system");
    }
}
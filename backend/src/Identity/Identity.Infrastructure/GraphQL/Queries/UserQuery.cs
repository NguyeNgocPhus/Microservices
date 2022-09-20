using Identity.Application.GraphQL.Resolvers;
using Identity.Infrastructure.GraphqQL.Queries;

namespace Identity.Infrastructure.GraphQL.Queries;

public class UserQuery : ObjectTypeExtension<RootQuery>
{
    protected override void Configure(IObjectTypeDescriptor<RootQuery> descriptor)
    {
        descriptor.Field("getToken")
            .Description("test query")
            .Type<StringType>()
            .ResolveWith<IUserResolver>(resolve => resolve.GetTokenAsync(default));
        descriptor.Field("getDetailUser")
            .Description("get user detail")
            .Argument("id", desc =>
            {
                desc.Description("User Id");
                desc.Type<LongType>();
            })
            .ResolveWith<IUserResolver>(resolve => resolve.GetUserByAdmin(default,default));
        descriptor.Field("testRollback")
            .Description("test roll back")
            .Argument("id", desc =>
            {
                desc.Description("User Id");
                desc.Type<LongType>();
            })
            .ResolveWith<IUserResolver>(resolve => resolve.TestRollBack(default,default));
        
    }
    
}
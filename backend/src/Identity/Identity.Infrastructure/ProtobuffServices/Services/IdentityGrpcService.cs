using Grpc.Core;
using Identity.Grpc.Internal.Services;

namespace Identity.Infrastructure.ProtobuffServices.Services;

public class IdentityGrpcService : IdentityService.IdentityServiceBase
{


    public override async Task<ImportUserResponse> ImportUser(ImportUserRequest request, ServerCallContext context)
    {
        await Task.CompletedTask;
        var a = request.Id;
        var meta = context.RequestHeaders;
        return new ImportUserResponse()
        {
            Status = "true"
        };
    }
}
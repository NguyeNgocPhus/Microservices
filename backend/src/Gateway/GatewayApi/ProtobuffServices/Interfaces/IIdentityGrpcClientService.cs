using Identity.Grpc.Internal.Services;

namespace GatewayApi.ProtobuffServices.Interfaces;

public interface IIdentityGrpcClientService
{
    Task<ImportUserResponse> ImportUserAsync(ImportUserRequest request, CancellationToken cancellationToken = default);
    
}
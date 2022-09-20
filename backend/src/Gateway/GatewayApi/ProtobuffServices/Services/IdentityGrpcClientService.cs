using System.Diagnostics;
using GatewayApi.gRPC;
using GatewayApi.ProtobuffServices.Interfaces;
using Grpc.Core;
using Grpc.Net.Client;
using Identity.Grpc.Internal.Services;
using Microsoft.Extensions.Options;
using Polly;

namespace GatewayApi.ProtobuffServices.Services;

public class IdentityGrpcClientService : IIdentityGrpcClientService
{
    
    private readonly Service _service;
    private readonly GrpcChannel _channel;


    public IdentityGrpcClientService(IOptions<MicroserviceConfiguration> configuration)
    {
        _service = configuration.Value.Services.First(x => x.Name.Equals("Identity"));
        _channel = GrpcChannel.ForAddress(_service.GrpcUrl);
    }

    public async Task<ImportUserResponse> ImportUserAsync(ImportUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var retryPolicy = Policy.Handle<Exception>().RetryAsync(_service.GrpcMaxRetries, onRetryAsync: async (exception, retry, context) =>
            {
                await Task.CompletedTask;
                Console.WriteLine($"retry lan {retry}");
            });
            return await retryPolicy.ExecuteAsync(async () =>
            {
                var sw = new Stopwatch();
                sw.Start();
                var client = new IdentityService.IdentityServiceClient(_channel);
                var result = await client.ImportUserAsync(new ImportUserRequest()
                {
                    Id = 1
                },headers: new Metadata()
                {
                    {"XRouter", "Gateway Module -> Identity Module"}
                }, cancellationToken: cancellationToken);
                 
                sw.Stop();
                return result;

            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
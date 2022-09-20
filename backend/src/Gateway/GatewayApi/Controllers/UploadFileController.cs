using System.Text.Json;
using GatewayApi.Attributes;
using GatewayApi.ProtobuffServices.Interfaces;
using GatewayApi.Securities.Authorization;
using Identity.Grpc.Internal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayApi.Controllers;


[ApiController]
[Route("[controller]/[action]")]
public class FileStorageController: ControllerBase
{
    private readonly IIdentityGrpcClientService _identityGrpcClient;
    private readonly ILogger<WeatherForecastController> _logger;

    public FileStorageController(ILogger<WeatherForecastController> logger, IIdentityGrpcClientService identityGrpcClient)
    {
        _logger = logger;
        _identityGrpcClient = identityGrpcClient;
    }
    
    [HttpGet]
    [Permissions(Permissions = new []{"Admin","Customer"})]
    [CacheFile]
    public async Task<IActionResult> UploadFileAsync()
    {
        // var result = await _identityGrpcClient.ImportUserAsync(new ImportUserRequest()
        // {
        //
        //     Id = 1
        // });
        
        return Ok("result");
    }
}
using GatewayApi;
using GatewayApi.gRPC;
using GatewayApi.Middleware;
using GatewayApi.ProtobuffServices.Interfaces;
using GatewayApi.ProtobuffServices.Services;
using GatewayApi.Securities.Authorization.Handlers;
using GatewayApi.Securities.Authorization.PolicyProviders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var builder = WebApplication.CreateBuilder(args);



// Clear default logger
//
// builder.Logging.ClearProviders();
// var logger = new LoggerConfiguration()
//     .WriteTo.Console()   
//     //.MinimumLevel.Verbose()
//     .CreateLogger();
// Log.Logger = logger;
// builder.Logging.AddSerilog(logger);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

Console.WriteLine($"\n>> ENV: {builder.Environment.EnvironmentName}");
foreach (var (key, value) in builder.Configuration.AsEnumerable())
{
    Console.WriteLine($"{key}-{value}");
}

Console.WriteLine($"<< ENV: {builder.Environment.EnvironmentName}\n");

builder.Services.AddControllers();
builder.Services.AddWebApi(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IIdentityGrpcClientService, IdentityGrpcClientService>();
builder.Services.Configure<MicroserviceConfiguration>(
    builder.Configuration.GetSection("Microservices"));
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

//
// builder.Services.AddMicroservice(builder.Configuration);

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseMiddleware<GateKeeperMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGraphQL();
});

try
{
    ThreadPool.SetMinThreads(10, 10);
    Log.Information("Starting web host");
    ThreadPool.SetMinThreads(200, 200);
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
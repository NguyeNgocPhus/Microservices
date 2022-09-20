using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GatewayApi.Common;
using GatewayApi.Common.Configuration;
using GatewayApi.gRPC;
using HotChocolate.Execution.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace GatewayApi;

public static class DependencyInjection
{
    public const string Identity = "Identity";
    public const string Inventory = "inventory";
    public const string Products = "products";
    public const string Reviews = "reviews";
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfiguration>(
            configuration.GetSection(nameof(JwtConfiguration)));
        services.AddMicroservice(configuration);
        services.AddGraphQLGateway(configuration);
        services.AddIdentityService(configuration);
        
        return services;
    }

    public static IServiceCollection AddMicroservice(this IServiceCollection services, IConfiguration configuration)
    {
        var msConfigs = new MicroserviceConfiguration();
        configuration.GetSection("Microservices").Bind(msConfigs);
        if (msConfigs.Services == null || msConfigs.Services.Count == 0)
        {
            throw new NullReferenceException("Microservices configuration is not correct");
        }
        
        foreach (var service in msConfigs.Services)
        {
            services.AddHttpClient(service.Name, (sp, client) =>
            {
                // Forward request headers from the client to microservices
                var context = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (context != null && context.Request.Headers.ContainsKey("Authorization"))
                {
                    client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(context?.Request.Headers["Authorization"]);
                }
        
                // client.DefaultRequestHeaders.Add(Nobisoft.Core.Constants.Tracker.Header.XRequestId, (string?) context?.Request.Headers[Nobisoft.Core.Constants.Tracker.Header.XRequestId]);
                // client.DefaultRequestHeaders.Add(Nobisoft.Core.Constants.Tracker.Header.XTraceIdentifier, context?.TraceIdentifier);
                client.BaseAddress = new Uri(service.Url);
            });
        }
        
        return services;

    }
    public static IServiceCollection AddGraphQLGateway(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfigs = new RedisConfiguration();
        configuration.GetSection("Redis").Bind(redisConfigs);
        var strConnection = $"{redisConfigs.Host}:{redisConfigs.Port},ssl={redisConfigs.UseSsl}";
        services.AddSingleton(ConnectionMultiplexer.Connect(strConnection));
        services
            .AddGraphQLServer()
            .AddTypeConverter<object, string>(from =>JsonSerializer.Serialize(from))
            //.AddHttpRequestInterceptor<HttpRequestInterceptor>()
            .SetRequestOptions(_ => new RequestExecutorOptions()
            {
                ExecutionTimeout = TimeSpan.FromMinutes(20)
            })
            .AddRemoteSchemasFromRedis("QS_GRAPHQL_GATEWAY_SCHEMA", sp => sp.GetRequiredService<ConnectionMultiplexer>())
            ;
        
        return services;
    }
    public static void AddIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(config =>
        {
            config.RequireHttpsMetadata = false;
            config.SaveToken = true;
            config.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = configuration["JwtConfiguration:Audience"],
                ValidIssuer =  configuration["JwtConfiguration:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfiguration:SymmetricSecurityKey"]))
            };
            config.Events = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var token = context.HttpContext.Request.Headers;
                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/notification/web")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
                OnTokenValidated = async context =>
                {
                    // To check token is valid and must be existing in the UserToken table in the database
                    // Once JWT is not existing in the UserToken table, the authentication process will be set as failed.

                    var userIdClaim =
                        context.Principal?.Claims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.UserId);
                    if (userIdClaim == null)
                    {
                        context.Fail("JWT Token does not contain User Id Claim.");
                    }
                    var token = context.HttpContext.Request.Headers["Authorization"].ToString()
                        .Replace("Bearer ", "");
                    // If we cannot get token from header, try to use from querystring (for wss)

                    // context.Fail("JWT Token does not contain User Id Claim.");
                    Console.WriteLine(@"Token Validated OK");
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    // Ensure we always have an error and error description.
                    if (string.IsNullOrEmpty(context.Error))
                        context.Error = "invalid_token";
                    if (string.IsNullOrEmpty(context.ErrorDescription))
                    {
                        // Pass the message from OnTokenValidated on method context.Fail(<message>)
                        if (context.AuthenticateFailure != null &&
                            context.AuthenticateFailure.Message.Length > 0)
                        {
                            context.ErrorDescription = context.AuthenticateFailure.Message;
                        }
                        else
                        {
                            // If we dont have error message from OnTokenValidated, set a message
                            context.ErrorDescription =
                                "This request requires a valid JWT access token to be provided.";
                        }
                    }

                    // Add some extra context for expired tokens.
                    if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() ==
                        typeof(SecurityTokenExpiredException))
                    {
                        var authenticationException =
                            context.AuthenticateFailure as SecurityTokenExpiredException;
                        context.Response.Headers.Add("WWW-Authenticate", "Bearer");
                        context.ErrorDescription = $"The token expired on {authenticationException?.Expires:o}";
                    }

                    return context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        status = 401,
                        error = context.Error,
                        errorDescription = context.ErrorDescription
                    }));
                }
            };
        });
    }
}
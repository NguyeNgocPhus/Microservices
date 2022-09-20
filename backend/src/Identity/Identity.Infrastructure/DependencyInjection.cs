using System.Reflection;
using AppAny.HotChocolate.FluentValidation;
using FluentValidation.AspNetCore;
using Identity.Application.Common.Configuration;
using Identity.Application.GraphQL.Resolvers;
using Identity.Application.Services.EventStore;
using Identity.Application.Services.Logger;
using Identity.Core.Aggregates;
using Identity.Infrastructure.Databases;
using Identity.Infrastructure.GraphQL.Mutations;
using Identity.Infrastructure.GraphQL.Queries;
using Identity.Infrastructure.GraphQL.Resolvers;
using Identity.Infrastructure.GraphqQL.Mutations;
using Identity.Infrastructure.GraphqQL.Queries;
using Identity.Infrastructure.Handlers.Events.Dynamic;
using Identity.Infrastructure.Rabbitmq.Interfaces;
using Identity.Infrastructure.Rabbitmq.Services;
using Identity.Infrastructure.Services.EventStore;
using Identity.Infrastructure.Services.Logger;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfiguration>(
            configuration.GetSection(nameof(JwtConfiguration)));
        services.Configure<RedisConfiguration>(
            configuration.GetSection("Redis"));
        services.Configure<EventBusConfiguration>(
            configuration.GetSection("EventBus"));
        
        var redisDb = new RedisConfiguration();
        configuration.GetSection("Redis").Bind(redisDb);
        
        services.AddFluentValidation(
            x => { x.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()); }
        );
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddMediatR(typeof(Infrastructure.DependencyInjection));
        services.AddSingleton<IEventBusSubscriptionsManager, EventBusSubscriptionsManager>();
        services.AddTransient<ApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IEventStoreService<UserAggregateRoot>, EventStoreService<UserAggregateRoot>>();
        services.AddScoped<ILoggerServices, LoggerServices>();
        services.AddSingleton<IDynamicEventHandler, TestDynamicEventHandler>();
        services.AddSingleton<IEventbus, Eventbus>();
        services
            .AddSingleton(ConnectionMultiplexer.Connect($"{redisDb.Host}:{redisDb.Port},ssl={redisDb.UseSsl}, abortConnect=false"))
            .AddGraphQLServer()
            .AddFluentValidation(x => { })
            .AddQueryType<RootQueryType>()
            .AddTypeExtension<UserQuery>()
            .AddMutationType<RootMutationType>()
            .AddTypeExtension<UserMutation>()
            .InitializeOnStartup()
            .PublishSchemaDefinition(c => c
                .SetName("Identity")
                //.IgnoreRootTypes()
                //.AddTypeExtensionsFromFile("./Stitching.graphql")
                .PublishToRedis("QS_GRAPHQL_GATEWAY_SCHEMA", sp => sp.GetRequiredService<ConnectionMultiplexer>()))
            ;
       

        services.AddSingleton<IUserResolver, UserResolver>();
        return services;
    }
}
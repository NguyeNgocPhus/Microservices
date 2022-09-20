using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Identity.Application.Common.Configuration;
using Identity.Application.Common.Constants;
using Identity.Application.Dto.Users;
using Identity.Application.Dto.Users.Response;
using Identity.Application.GraphQL.ObjectType.Users;
using Identity.Application.GraphQL.Resolvers;
using Identity.Application.Services.EventStore;
using Identity.Core.Aggregates;
using Identity.Core.Exceptions;
using Identity.Infrastructure.Databases;
using Identity.Infrastructure.Events;
using Identity.Infrastructure.Rabbitmq;
using Identity.Infrastructure.Rabbitmq.Interfaces;
using Infrastructure.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nobisoft.Core.Interfaces;

namespace Identity.Infrastructure.GraphQL.Resolvers;

public class UserResolver : IUserResolver
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly JwtConfiguration _jwtOption;
    private readonly IEventbus _eventbus;
    private readonly ISnowflakeIdService _snowflakeId;
    private readonly IEventStoreService<UserAggregateRoot> _eventStoreService;
    private readonly ApplicationDbContext _dbContext;


    public UserResolver(IMediator mediator, IMapper mapper, IOptions<JwtConfiguration> jwtOption, IEventbus eventbus, ISnowflakeIdService snowflakeId, IEventStoreService<UserAggregateRoot> eventStoreService, ApplicationDbContext dbContext)
    {
        _mediator = mediator;
        _mapper = mapper;
        _eventbus = eventbus;
        _snowflakeId = snowflakeId;
        _eventStoreService = eventStoreService;
        _dbContext = dbContext;
        _jwtOption = jwtOption.Value;
    }

    public async Task<UserDto> GetUserByAdmin(long id, CancellationToken cancellationToken)
    {

        var userAggregateRoot = new UserAggregateRoot(id);
        var userAggregate = await _eventStoreService.AggregateStreamAsync(Application.Common.Constants.EventStore.Direction.Forward, userAggregateRoot.StreamName, long.MaxValue, int.MaxValue, cancellationToken);
        
        
        return new UserDto();
    }
    public async Task<CreateUserResponse> CreateUserAsync(CreateUserObjectType objectType, CancellationToken cancellationToken = default)
    {
        var userId = await _snowflakeId.GenerateId();
        var userAggregateRoot = new UserAggregateRoot(userId);
        // _eventbus.Publish(new IntegrationEvent(Guid.NewGuid(), DateTimeOffset.UtcNow));
        var userAggregate = userAggregateRoot.Initialize
        (
            await _snowflakeId.GenerateId(),
            objectType.Name,
            objectType.Email,
            objectType.Password,
            await _snowflakeId.GenerateId(),
            DateTimeOffset.UtcNow,
            await _snowflakeId.GenerateId()
        );
        await _eventStoreService.StartStreamAsync(userAggregate.StreamName, userAggregate, cancellationToken);
        // var user = _mapper.Map<UserReadModel>(userAggregate);
        //
        // await _dbContext.AddAsync(user, cancellationToken);
        // await _dbContext.SaveChangesAsync(cancellationToken);
        // var userEvent = new InitializedUserEvent(
        //     userAggregate.Id,
        //     userAggregate.Name,
        //     userAggregate.Email,
        //     userAggregate.Password,
        //     await _snowflakeId.GenerateId(),
        //     DateTimeOffset.UtcNow,
        //     await _snowflakeId.GenerateId()
        //     );
        // await _mediator.Publish(userEvent, cancellationToken);
        return new CreateUserResponse
        {
            Id = userAggregate.Id,
            Success = true
        };
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _eventbus.Publish(new TestEvent
            {
                Name = "Name",
                Email = "Email",
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow
                
                
            });
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Email, "phu@gmail.com"),
                new Claim(JwtClaimTypes.Name, "Phu"),
                new Claim(JwtClaimTypes.UserId, Guid.NewGuid().ToString()), 
                new Claim(JwtClaimTypes.Permission, "SUPPERADMIN")
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOption.SymmetricSecurityKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOption.Audience,
                Issuer = _jwtOption.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_jwtOption.Expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            await Task.CompletedTask;
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
           
            return token;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<UserDto> DeleteUserAsync(long id ,CancellationToken cancellationToken = default)
    {
        try
        {
            var userAggregateRoot = new UserAggregateRoot(id);
            var userAggregate = await _eventStoreService.AggregateStreamAsync(EventStore.Direction.Forward, userAggregateRoot.StreamName, long.MaxValue, int.MaxValue, cancellationToken);

            userAggregate.DeleteUser(userAggregateRoot.Id,true,await _snowflakeId.GenerateId() , DateTimeOffset.UtcNow, await _snowflakeId.GenerateId());
            
            await _eventStoreService.AppendStreamAsync(userAggregate.StreamName, userAggregate, cancellationToken);
            return new UserDto();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<UpdateUserResponse> UpdateUserAsync(UpdateUserObjectType objectType,CancellationToken cancellationToken = default)
    {
        try
        {
            var userAggregateRoot = new UserAggregateRoot(objectType.Id);
            var userAggregate = await _eventStoreService.AggregateStreamAsync(EventStore.Direction.Forward, userAggregateRoot.StreamName, long.MaxValue, int.MaxValue, cancellationToken);
            userAggregate.UpdatedUser(
                userAggregateRoot.Id,
                objectType.Name,
                objectType.Email,
                await _snowflakeId.GenerateId(),
                DateTimeOffset.UtcNow,
                await _snowflakeId.GenerateId()
            );
            await _eventStoreService.AppendStreamAsync(userAggregateRoot.StreamName, userAggregate, cancellationToken);
            return new UpdateUserResponse
            {
                Success = true,
                Id = userAggregateRoot.Id
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    // test roll back
    public async Task<UserDto> TestRollBack(long id,CancellationToken cancellationToken = default)
    {
        try
        {
            
            var userAggregateRoot = new UserAggregateRoot(id);
            var userAggregate = await _eventStoreService.AggregateStreamAsync(EventStore.Direction.Forward, userAggregateRoot.StreamName, long.MaxValue, int.MaxValue, cancellationToken);

            var eventRollback = userAggregate.EventHistories.FirstOrDefault(x => x.EventType.Contains("InitializedUserEvent"));
            var oldData = await _eventStoreService.AggregateStreamAsync(EventStore.Direction.Forward, userAggregateRoot.StreamName, eventRollback.EventRevision, int.MaxValue, cancellationToken);
            var response = _mapper.Map<UserDto>(oldData);
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}
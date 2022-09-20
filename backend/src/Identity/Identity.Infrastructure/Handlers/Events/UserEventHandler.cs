using AutoMapper;
using Identity.Core.Events.User;
using Identity.Core.ReadModels;
using Identity.Infrastructure.Databases;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Handlers.Events;

public class UserEventHandler: 
    INotificationHandler<InitializedUserEvent>,
    INotificationHandler<UpdateUserEvent>,
    INotificationHandler<DeletedUserEvent>
{
    public readonly ApplicationDbContext _dbContext;
    public readonly IMapper _mapper;

    public UserEventHandler(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task Handle(InitializedUserEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            var user =  _mapper.Map<UserReadModel>(@event);
            await _dbContext.Users.AddAsync(user,cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
    }

    public async Task Handle(UpdateUserEvent  @event, CancellationToken cancellationToken)
    {
        try
        { 
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == @event.Id, cancellationToken);
            user.Email = @event.Email;
            user.Name = @event.Name;
            user.ModifiedBy = @event.ModifiedBy;
            user.ModifiedTime = @event.ModifiedTime;
        
            await _dbContext.SaveChangesAsync(cancellationToken);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
    }

    public async Task Handle(DeletedUserEvent  @event, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == @event.Id, cancellationToken);
            user.Deleted = @event.Deleted;
            user.ModifiedBy = @event.ModifiedBy;
            user.ModifiedTime = @event.ModifiedTime;
        
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
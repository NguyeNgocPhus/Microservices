using System.Reflection;
using System.Runtime.CompilerServices;
using Identity.Application.Services.EventStore;
using Identity.Core.Aggregates;
using Identity.Core.Events;
using Identity.Infrastructure.Databases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nobisoft.Core.Exceptions;
using Nobisoft.Core.Interfaces;

namespace Identity.Infrastructure.Services.EventStore;

public class EventStoreService<TAggregate> : IEventStoreService<TAggregate>
{
    private readonly IMediator _mediator;
    private readonly ISnowflakeIdService _snowflakeIdService;
    private readonly ApplicationDbContext _dbContext;

    public EventStoreService(IMediator mediator, ISnowflakeIdService snowflakeIdService, ApplicationDbContext dbContext)
    {
        _mediator = mediator;
        _snowflakeIdService = snowflakeIdService;
        _dbContext = dbContext;
    }

    public async Task StartStreamAsync(string streamName, TAggregate aggregate, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (!(aggregate is AggregateRoot agg))
            throw new InvalidOperationException("Aggregate should not be null");
        foreach ( EventBase domainEvent in (ICollection<EventBase>) agg.DomainEvents)
        {
            await _dbContext.Events.AddAsync(new Event(streamName,domainEvent.Revision,domainEvent.GetType().AssemblyQualifiedName,JsonConvert.SerializeObject((object) domainEvent),domainEvent.CreatedBy));
            await _mediator.Publish(domainEvent, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AppendStreamAsync(string streamName, TAggregate aggregate, CancellationToken cancellationToken = default(CancellationToken))
    {
        Event? stream = await _dbContext.Events.Where(x => x.StreamName == streamName).FirstOrDefaultAsync(cancellationToken);
        if (stream == null)
            throw new StreamNotFoundException(streamName, nameof (AppendStreamAsync), "Stream not found");
        if (!(aggregate is AggregateRoot agg))
            throw new InvalidOperationException("Aggregate should not be null");
        foreach ( EventBase domainEvent in (ICollection<EventBase>) agg.DomainEvents)
        {
            await _dbContext.Events.AddAsync(new Event(streamName,domainEvent.Revision,domainEvent.GetType().AssemblyQualifiedName,JsonConvert.SerializeObject((object) domainEvent),domainEvent.CreatedBy));
            await _mediator.Publish(domainEvent, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    

    public async Task<TAggregate> AggregateStreamAsync(Application.Common.Constants.EventStore.Direction direction,string streamName, long revision, int maxCount = 2147483647, CancellationToken cancellationToken = default(CancellationToken))
    {
        IQueryable<Event> query = _dbContext.Events.Where(x => x.StreamName == streamName);
        Application.Common.Constants.EventStore.Direction direction1 = direction;
        switch (direction1)
        {
            case Application.Common.Constants.EventStore.Direction.Backward:
                query = query.Where(x => x.Revision >= revision).OrderByDescending(x => x.Revision);
                break;
            case Application.Common.Constants.EventStore.Direction.Forward:
                query = query.Where(x => x.Revision <= revision).OrderBy(x => x.Revision);
                break;
            
        }
        List<Event> events = await query.Take<Event>(maxCount).ToListAsync<Event>(cancellationToken);
        
        if (events.Count == 0)
            throw new StreamNotFoundException(streamName, "AppendStreamAsync", "Stream " + streamName + "  not found");
        object instance = RuntimeHelpers.GetUninitializedObject(typeof (TAggregate));
        try
        {
            foreach (Event @event in events)
            {
                object obj = FromByteArray(@event.Type, @event.Data);
                Type type = Type.GetType(@event.Type);
                MethodInfo x = !(type == null)
                    ? instance.GetType().GetMethod("Apply", BindingFlags.Instance | BindingFlags.Public|BindingFlags.NonPublic, (Binder) null,new Type[3]
                    {
                        type,
                        typeof(long),
                        typeof(long)
                    },(ParameterModifier[]) null)
                    : throw new BadRequestException("sau");
                x?.Invoke(instance, new object[3]
                {
                    obj,
                    (object)@event.Revision,
                    @event.CreatedBy
                });
                var a = instance;
                obj = (object) null;
                type = (Type) null;
                x = (MethodInfo) null;
            }
           
        }
        catch (Exception ex)
        {
           
            throw ex;
        }
        return (TAggregate) instance;
    }

    private object FromByteArray(string name, string data)
    {
        Type type = Type.GetType(name);
        return JsonConvert.DeserializeObject(data, type);
    }
    
}
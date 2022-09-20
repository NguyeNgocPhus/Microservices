using Identity.Core.Events;
using Identity.Core.Events.User;

namespace Identity.Core.Aggregates;

public class UserAggregateRoot  : AggregateRoot
{
    public long Id { get; set; }
    public string Email { get; set; }
     
    public string Name { get; set; }
    public string Password { get; set; }
    public long CreatedBy { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public bool Deleted { get; set; } = false;
    public long? ModifiedBy { get; set; }
    public DateTimeOffset? ModifitedTime { get; set; }
    
    public string StreamName => $"User-{Id}";

    public UserAggregateRoot(long id)
    {
        Id = id;
    }

    public UserAggregateRoot Initialize(long id,string name, string email, string password, long createdBy, DateTimeOffset createdTime,long revision)
    {
        var @event = new InitializedUserEvent(id,name,email,password,createdBy,createdTime,revision) ;
        AddDomainEvent(@event);
        Apply(@event, revision,createdBy);
        return this;
    }

    public void Apply(InitializedUserEvent @event, long revision,long? createdBy)
    {
        Id = @event.Id;
        Email = @event.Email;
        Password = @event.Password;
        CreatedBy = @event.CreatedBy;
        CreatedTime = @event.CreatedTime;
        Name = @event.Name;
        EventHistories ??= new List<EventHistoryItem>();
        EventHistories.Add(new EventHistoryItem()
        {
            CreatedBy = createdBy ?? -1,
            EventRevision = revision,
            EventType = @event.GetType().AssemblyQualifiedName?? string.Empty
        });
    }
    public UserAggregateRoot DeleteUser(long id , bool deleted, long modifiedBy, DateTimeOffset modifiedTime,long revision)
    {
        var @event = new DeletedUserEvent(id,deleted,modifiedBy,modifiedTime,revision) ;
        AddDomainEvent(@event);
        Apply(@event, revision,modifiedBy);
        return this;
    }
    public void Apply(DeletedUserEvent @event, long revision,long? createdBy)
    {
        Deleted = @event.Deleted;
        ModifiedBy = @event.ModifiedBy;
        ModifitedTime = @event.ModifiedTime;
        EventHistories ??= new List<EventHistoryItem>();
        EventHistories.Add(new EventHistoryItem()
        {
            CreatedBy = createdBy ?? -1,
            EventRevision = revision,
            EventType = @event.GetType().AssemblyQualifiedName?? string.Empty
        });
        
        
    }
    
    public UserAggregateRoot UpdatedUser(long id , string name, string email, long modifiedBy, DateTimeOffset modifitedTime,long revision)
    {
        var @event = new UpdateUserEvent(id,name,email,modifiedBy,modifitedTime,revision) ;
        AddDomainEvent(@event);
        Apply(@event, revision, modifiedBy);
        return this;
    }
    public void Apply(UpdateUserEvent @event, long revision,long? createdBy)
    {
        Name = @event.Name;
        Email = @event.Email;
        ModifiedBy = @event.ModifiedBy;
        ModifitedTime = @event.ModifiedTime;
        EventHistories ??= new List<EventHistoryItem>();
        EventHistories.Add(new EventHistoryItem()
        {
            CreatedBy = createdBy ?? -1,
            EventRevision = revision,
            EventType = @event.GetType().AssemblyQualifiedName?? string.Empty
        });
        
    }
    
    
    
}
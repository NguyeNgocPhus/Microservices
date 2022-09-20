using Identity.Core.Events;

namespace Identity.Core.Aggregates;

public class AggregateRoot
{
    private List<EventBase> _domainEvents = new List<EventBase>();
    public IReadOnlyCollection<EventBase> DomainEvents => (IReadOnlyCollection<EventBase>) _domainEvents.AsReadOnly();

    public List<EventHistoryItem> EventHistories = new List<EventHistoryItem>();
    public void AddDomainEvent(EventBase @event)
    {
        if (_domainEvents == null)
            _domainEvents = new List<EventBase>();
        _domainEvents.Add(@event);
      
        
    }
    
    public void RemoveDomainEvent(EventBase @event)
    {
        _domainEvents.Remove(@event);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
}

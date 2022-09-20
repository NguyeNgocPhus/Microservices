using Identity.Infrastructure.Rabbitmq.Interfaces;

namespace Identity.Infrastructure.Rabbitmq.Services;

public class EventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    private readonly Dictionary<string, Type> _handlers;

    public EventBusSubscriptionsManager()
    {
        _handlers = new Dictionary<string, Type>();
    }

    public void AddDynamicSubscription<IHandler>(string eventName)
    {
        _handlers.Add(eventName,typeof(IHandler));
    }

    public bool HasSubscriptionsForEvent(string eventName)
    {
        return _handlers.ContainsKey(eventName);
    }

    public Type GetHandlersForEvent(string eventName)
    {
        return _handlers[eventName];
    }
}
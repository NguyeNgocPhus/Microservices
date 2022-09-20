namespace Identity.Infrastructure.Rabbitmq.Interfaces;

public interface IEventBusSubscriptionsManager
{
    void AddDynamicSubscription<IHandler>(string eventName);

    bool HasSubscriptionsForEvent(string eventName);
    Type GetHandlersForEvent(string eventName);
}
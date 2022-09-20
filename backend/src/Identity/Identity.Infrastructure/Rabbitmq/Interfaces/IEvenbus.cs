namespace Identity.Infrastructure.Rabbitmq.Interfaces;

public interface IEventbus
{
    
    void Publish(IntegrationEvent @event);

    void SubscribeDynamic<TIntegrationEventHandler>(string eventName);

}
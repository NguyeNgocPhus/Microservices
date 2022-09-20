namespace Identity.Infrastructure.Rabbitmq.Interfaces;

public interface IDynamicEventHandler
{
    Task Handle(object eventData);
}
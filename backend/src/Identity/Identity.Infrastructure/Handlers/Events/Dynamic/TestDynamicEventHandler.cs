using Identity.Infrastructure.Rabbitmq.Interfaces;

namespace Identity.Infrastructure.Handlers.Events.Dynamic;

public class TestDynamicEventHandler :IDynamicEventHandler
{
    public Task Handle(object eventData)
    {
        throw new NotImplementedException();
    }
}
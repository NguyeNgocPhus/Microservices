using Identity.Infrastructure.Rabbitmq;

namespace Identity.Infrastructure.Events;

public class TestEvent : IntegrationEvent
{
    public string Name { get; set; }
    public string Email { get; set; }
    
}
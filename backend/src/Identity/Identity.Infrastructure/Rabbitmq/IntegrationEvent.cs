namespace Identity.Infrastructure.Rabbitmq;

public class IntegrationEvent
{
    
    public Guid Id { get;  set; }
    
    public DateTimeOffset Created { get;  set; }

    
}
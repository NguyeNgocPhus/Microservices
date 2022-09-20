namespace Identity.Application.gRPC;

public class MicroserviceConfiguration
{
    public MicroserviceConfiguration(ICollection<Service> services) => Services = services;

    public MicroserviceConfiguration()
    {
    }
    public ICollection<Service> Services { get; set; }
}
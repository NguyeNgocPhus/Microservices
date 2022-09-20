namespace GatewayApi.gRPC;

public class Service
{
    public string Name { get; set; }

    public string Url { get; set; }

    public string GrpcUrl { get; set; }

    public int GrpcMaxRetries { get; set; }

    public int GrpcDeadline { get; set; }
    public Service(string name,string url, string grpcUrl, int grpcMaxRetries, int grpcDeadline)
    {
        Name = name;
        Url = url;
        GrpcUrl = grpcUrl;
        GrpcMaxRetries = grpcMaxRetries;
        GrpcDeadline = grpcDeadline;
    }
    public Service()
    {
      
    }
}
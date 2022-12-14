namespace Identity.Application.Commom.Configuration;

public class AppDbConfiguration
{
    public string Server { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public string Scheme { get; set; }
    public string Database { get; set; }
    public string Port { get; set; }
}
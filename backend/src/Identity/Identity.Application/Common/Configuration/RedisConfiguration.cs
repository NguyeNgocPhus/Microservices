namespace Identity.Application.Common.Configuration;

public class RedisConfiguration
{
    public string Host { get; set; }

    public string Port { get; set; }

    public bool SkipPassword { get; set; } = false;

    public bool EnableReplication { get; set; } = false;

    public string Password { get; set; }

    public string UseSsl { get; set; }

    public string User { get; set; }

    public string ClientName { get; set; } = "Module Name";

    public int DatabaseNumber { get; set; } = 0;
}
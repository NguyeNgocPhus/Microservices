using System.Runtime.CompilerServices;
using Identity.Application.Services.Logger;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;

namespace Identity.Infrastructure.Services.Logger;

public class LoggerServices:ILoggerServices
{
    private readonly ILogger<LoggerServices> _logger;

    public LoggerServices(ILogger<LoggerServices> logger)
    {
        _logger = logger;
    }

    public void LogFatal(Exception exception, string message, string caller = "", string callerFilePath = "",int callerLineNumber = 0, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void LogFatal(string message, [CallerMemberName] string caller = "",[CallerFilePath] string callerPath = "",[CallerLineNumber] int line = 0, params object[] args)
    {
        using (GlobalLogContext.Lock())
        {
            GlobalLogContext.PushProperty("CallerMemberName", (object) caller, true);
            GlobalLogContext.PushProperty("CallerFilePath", (object) callerPath, true);
            GlobalLogContext.PushProperty("CallerLineNumber", (object) line, true);
            Log.Fatal(message, args);
        }
    }

    public void LogDebug(Exception exception, string message, string caller = "", string callerFilePath = "", int callerLineNumber = 0, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void LogDebug(string message, string caller = "", string callerFilePath = "", int callerLineNumber = 0, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void LogError(Exception exception, string message, string caller = "", string callerFilePath = "", int callerLineNumber = 0, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void LogError(string message, string caller = "", string callerFilePath = "", int callerLineNumber = 0, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void LogInformation(Exception exception, string message, string caller = "", string callerFilePath = "", int callerLineNumber = 0, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void LogInformation(string message, string caller = "", string callerFilePath = "", int callerLineNumber = 0, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void LogWarning(Exception exception, string message, string caller = "", string callerFilePath = "", int callerLineNumber = 0, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void LogWarning(string message, string caller = "", string callerFilePath = "", int callerLineNumber = 0, params object[] args)
    {
        throw new NotImplementedException();
    }
}
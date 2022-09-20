using Identity.Application.Extensions;
using Identity.Application.Services.Logger;

namespace Identity.Infrastructure.Filters;

public class GraphQLErrorFilter: IErrorFilter
{
    private readonly ILoggerServices _loggerService;

    public GraphQLErrorFilter(ILoggerServices loggerService)
    {
        _loggerService = loggerService;
    }

    public IError OnError(IError error)
    {
        if (error.Exception == null && error.Code != null)
        {
            // TODO: When this should happen?
            // _loggerService.LogError($"{nameof(GraphQLErrorFilter)} >> {error.Message} <{error.Code}> with request id {_httpContextAccessorService.Tracker().RequestId} +0 ms");
            return error.WithMessage($"{error.Exception?.Message ?? error.Message} <{error.Code}> with request id  +0 ms");
        }
        
        // var tracker = error.Exception?.Data["Tracker"] as Tracker;

        error = error.BuildError();
        _loggerService.LogFatal($"{nameof(GraphQLErrorFilter)} >> {error.Exception?.Message ?? string.Empty} <{error.Code}> with request id  +0 ms");
        // Console.WriteLine($"{nameof(GraphQLErrorFilter)} >> {error.Exception?.Message ?? string.Empty} <{error.Code}> with request id  +0 ms");
        return error.WithMessage(error.Exception?.Message ?? error.Message);
    }
}
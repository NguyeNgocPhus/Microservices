

namespace GatewayApi;

/// <summary>
/// Handle GraphQL error globally
/// </summary>
public class GraphQLErrorFilter : IErrorFilter
{
    
    /// <summary>
    /// All exceptions must go here
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public IError OnError(IError error)
    {
        // TODO: Should clear sensitive data from here
        if (error.Exception == null && error.Code != null)
        {
            // _loggerService.LogError($"{nameof(GraphQLErrorFilter)} >> {error.Message} <{error.Code}> with request id +0 ms");
            return error.WithMessage(error.Exception?.Message ?? error.Message);
        }

        error = error
            //.WithCode($"MUST HANDLE BY EXCEPTION TYPE <{error.Exception?.GetType().Name}>")
            .WithCode(GetRemoteErrorCodeIfAny(error) ?? "000500")
            .RemoveExtension("stackTrace")
            .RemoveLocations()
            .RemoveExtension("remote")
            .RemoveExtension("message");

        // _loggerService.LogError($"{nameof(GraphQLErrorFilter)} >> {error.Exception?.Message ?? string.Empty} <{error.Code}> with request id {_httpContextAccessorService.Tracker().RequestId} +0 ms");
        return error.WithMessage(error.Exception?.Message ?? error.Message);
    }

    private static string? GetRemoteErrorCodeIfAny(IError error)
    {
        // Only handle if we have remote key
        if (error.Extensions == null || !error.Extensions.ContainsKey("remote"))
            return null;
        var remote = error.Extensions.FirstOrDefault(e => e.Key.ToLower().Equals("remote"));
        // Get remote extensions
        var remoteValue = remote.Value as Error;
        if (remoteValue?.Extensions == null)
            return null;
        var (_, value) = remoteValue.Extensions.FirstOrDefault(x => x.Key.ToLower().Equals("code"));
        // if (value == null || (value as string).IsNullOrEmpty())
        //     return null;
        return value as string;
    }
}
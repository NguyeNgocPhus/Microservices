using System.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;

namespace GatewayApi.Middleware;

public class GateKeeperMiddleware
{
    private readonly RequestDelegate _next;



    public GateKeeperMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Check the request before move to the next function in the pipeline
            var hasXRequestId = context.Request.Headers.ContainsKey("X-Request-Id");
            if (!hasXRequestId)
            {
                context.Request.Headers["X-Request-Id"] = Guid.NewGuid().ToString();
            }

            // Start the stop watch
            var sw = new Stopwatch();
            sw.Start();
            // Observe the response event
            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext) state;
                // Write the response back to client
                httpContext.Response.Headers.Add("RequestId", context.Request.Headers["X-Request-Id"]);
                httpContext.Response.Headers.Add("TraceIdentifier", context.TraceIdentifier);
                sw.Stop();
                httpContext.Response.Headers.Add("ExecutionTime", $"+{sw.ElapsedMilliseconds.ToString()} ms");
                Console.WriteLine($"{httpContext.Request.Protocol} {httpContext.Request.Method.ToUpper()} {httpContext.Response.StatusCode} {httpContext.Request.GetDisplayUrl()} with request id {context.Request.Headers["X-Request-Id"]} (+{sw.ElapsedMilliseconds} ms)");
                return Task.CompletedTask;
            }, context);
            // // Validate jwt if presented
            // var jwtTokenWithBearer = _httpContextAccessorService.GetRequestHeader("Authorization");
            // if (jwtTokenWithBearer != null && jwtTokenWithBearer.Replace("Bearer", "").Trim().Length > 0)
            // {
            //     var jwt = jwtTokenWithBearer.Replace("Bearer", "").Trim();
            //     var result = await _identityClientGrpcService.ValidateJwtAsync(jwt);
            //     if (!result.IsAuthenticated)
            //         throw new UnauthorizedJwtException(nameof(GateKeeperMiddleware), nameof(InvokeAsync), $"Unable to authenticate your request with error code {result.ErrorCode}", _httpContextAccessorService.Tracker());
            //     // Go next here
            // }


            // Call the next function in the pipeline
            await _next(context);
        }
        
        catch (Exception ex)
        {
            // Handle exception if any
            var exception = new Exception(ex.Message);
            if (ex.Data.Keys.Cast<string>().Any(k => k.Equals("Tracker")))
            {
                exception.Data["Tracker"] = ex.Data["Tracker"];
            }
            throw exception;
        }
    }
}
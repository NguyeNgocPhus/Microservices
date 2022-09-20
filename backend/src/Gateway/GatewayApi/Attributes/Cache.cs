using Microsoft.AspNetCore.Mvc.Filters;

namespace GatewayApi.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CacheFileAttribute :  Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var a = 1;
        var c = 2;
        var b = await next();
        var d = 1;


    }
}
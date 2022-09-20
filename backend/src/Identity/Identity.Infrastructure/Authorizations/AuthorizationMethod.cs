using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Authorizations;

public static class AuthorizationMethod
{

    public static IObjectFieldDescriptor RequiredInternalRoles(this IObjectFieldDescriptor descriptor,
        string[]? roles = null)
    {
        descriptor.Use((next => async conext =>
        {
            var httpContextAccessorService = conext.Services.GetRequiredService<IHttpContextAccessor>();
            string token = httpContextAccessorService.HttpContext?.Request.Headers["Authorization"] ?? throw new InvalidOperationException();
            
            await next(conext);
        }));
        return descriptor;
    }
        
    
    
}
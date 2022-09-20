using GatewayApi.Securities.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace GatewayApi.Securities.Authorization.PolicyProviders;

public class PermissionPolicyProvider:IAuthorizationPolicyProvider
{
    public const string PermissionsGroup = "Permissions";
    private DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }


    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="options"></param>
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        // Set fallback policy provider as default policy
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    /// <summary>
    /// Returns the default authorization policy (the policy used for [Authorize] attributes without a policy specified).
    /// </summary>
    /// <returns></returns>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        // You may leave it empty to use your custom authorization with the default authentication scheme
        //return Task.FromResult(new AuthorizationPolicyBuilder("SchemeName").RequireAuthenticatedUser().Build());
        return Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }

    /// <summary>
    /// Returns the fallback authorization policy (the policy used by the Authorization Middleware when no policy is specified).
    /// </summary>
    /// <returns></returns>
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();
    
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName))
        {
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
        var policyTokens = policyName.Split('$', StringSplitOptions.RemoveEmptyEntries);
        
        if (policyTokens?.Any() != true)
        {
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
        
        if (policyName.StartsWith(PermissionsGroup, StringComparison.OrdinalIgnoreCase))
        {
            var policy = new AuthorizationPolicyBuilder();
            // Split and transform string policy from PermissionsAttribute to requirement then add to the policy
            policy.AddRequirements(new PermissionsRequirement(policyTokens[1]));
            return Task.FromResult(policy.Build());
        }
        

        return Task.FromResult<AuthorizationPolicy>(null);
    }
    
}
using GatewayApi.Securities.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace GatewayApi.Securities.Authorization.Handlers;

public class PermissionAuthorizationHandler:AuthorizationHandler<PermissionsRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
    {
        try
        {
            // If the user is not authenticated first, do nothing and return
            if (context.User.Identity != null && !context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }
            // If the user was already authorized, do nothing and return
            if (context.HasSucceeded)
            {
                return Task.CompletedTask;
            }
            
            // Get the requirements based on the format "R1|R2|R3|RN" and mandate having at least 1 of them
            var requirementTokens = requirement.Permissions.Split("|", StringSplitOptions.RemoveEmptyEntries);

            if (requirementTokens?.Any() == false)
            {
                return Task.CompletedTask;
            }
            // Create a list of requirements of format "application:area:permission", if any of them fail in format
            // or if there isn't at least one proper requirement, do nothing and return
            
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
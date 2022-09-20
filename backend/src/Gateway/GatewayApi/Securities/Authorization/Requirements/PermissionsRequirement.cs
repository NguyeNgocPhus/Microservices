using Microsoft.AspNetCore.Authorization;

namespace GatewayApi.Securities.Authorization.Requirements;

public class PermissionsRequirement: IAuthorizationRequirement
{
    public string Permissions { get; }
    public PermissionsRequirement(string permissions)
    {
        Permissions = permissions;
    }

}
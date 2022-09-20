using Microsoft.AspNetCore.Authorization;

namespace GatewayApi.Securities.Authorization;

public class PermissionsAttribute: AuthorizeAttribute
{
    public const string PermissionsGroup = "Permissions";
    private string[] _permissions;
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///
    /// Sample
    /// 
    ///     [Permissions(
    ///                 Permissions = new[] { "weather:*:*"},
    ///                 Roles = new [] { "user", "super admin"},
    ///                 Scopes = new[] { "App.Demo" })
    ///     ]
    /// 
    /// Note:
    ///
    ///     If we leave it as blank ([Permissions]), it work like [Authorize] attribute
    /// </remarks>
    public PermissionsAttribute()
    {
        _permissions = Array.Empty<string>();
    }
    
    /// <summary>
    ///  Array of permissions to be evaluated by authorizer
    /// </summary>
    public string[] Permissions
    {
        get => _permissions;
        set => BuildPolicy(ref _permissions, value, PermissionsGroup);
    }
    private bool _isDefault = true;
    private void BuildPolicy(ref string[] target, string[] value, string group)
    {
        target = value;

        if (_isDefault)
        {
            Policy = string.Empty;
            _isDefault = false;
        }
        var v = $"{group}${string.Join("|", target)}";
        Policy += $"{group}${string.Join("|", target)};";
    }
}
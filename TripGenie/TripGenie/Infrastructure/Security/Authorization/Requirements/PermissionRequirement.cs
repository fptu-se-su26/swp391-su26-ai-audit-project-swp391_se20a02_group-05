using Microsoft.AspNetCore.Authorization;

namespace TripGenie.API.Infrastructure.Security.Authorization.Requirements;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

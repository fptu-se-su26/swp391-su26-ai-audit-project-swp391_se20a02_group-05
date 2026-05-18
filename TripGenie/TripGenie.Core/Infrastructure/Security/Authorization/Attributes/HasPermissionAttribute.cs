using Microsoft.AspNetCore.Authorization;

namespace TripGenie.API.Infrastructure.Security.Authorization.Attributes;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission)
    {
    }
}

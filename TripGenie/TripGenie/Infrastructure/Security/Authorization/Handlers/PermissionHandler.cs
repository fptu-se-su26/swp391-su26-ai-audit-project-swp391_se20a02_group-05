using Microsoft.AspNetCore.Authorization;
using TripGenie.API.Infrastructure.Security.Authorization.Requirements;

namespace TripGenie.API.Infrastructure.Security.Authorization.Handlers;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        // 1. Check if user is Super Admin (has *:*:*)
        if (context.User.HasClaim("permissions", "*:*:*"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 2. Get permissions from Claims (already loaded during authentication/authorization middleware)
        var permissions = context.User.FindAll("permissions").Select(x => x.Value);

        if (EvaluatePermission(permissions, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Evaluates if the user's permissions satisfy the required permission.
    /// Supports wildcard matching (e.g., 'identity:*' matches 'identity:user:delete').
    /// </summary>
    /// <param name="userPermissions">List of permissions assigned to the user.</param>
    /// <param name="requiredPermission">The permission required by the resource.</param>
    /// <returns>True if access is granted; otherwise, false.</returns>
    private static bool EvaluatePermission(IEnumerable<string> userPermissions, string requiredPermission)
    {
        // Optimization: Exact match check
        if (userPermissions.Contains(requiredPermission)) return true;

        var requiredParts = requiredPermission.Split(':');

        foreach (var userPerm in userPermissions)
        {
            // Already checked *:*:* in HandleRequirementAsync, but for completeness:
            if (userPerm == "*:*:*") return true;

            var userParts = userPerm.Split(':');
            
            // Logic Wildcard matching
            // Rule: i-th part must match, OR user has '*' at i-th part (which matches everything from here down)
            
            bool isMatch = true;
            for (int i = 0; i < userParts.Length; i++)
            {
                if (userParts[i] == "*") return true; 

                if (i >= requiredParts.Length || userParts[i] != requiredParts[i])
                {
                    isMatch = false;
                    break;
                }
            }

            // If we reached here without a wildcard return, it's a match only if lengths are same
            if (isMatch && userParts.Length == requiredParts.Length) return true;
        }

        return false;
    }
}


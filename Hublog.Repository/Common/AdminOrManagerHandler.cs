using Hublog.Repository.Common;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

public class AdminOrManagerHandler : AuthorizationHandler<AdminOrManagerRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrManagerRequirement requirement)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value; // Get role
        var managerStatusClaim = context.User.FindFirst("ManagerStatus")?.Value; // Get ManagerStatus

        bool isManager = bool.TryParse(managerStatusClaim, out bool result) && result; // Convert to bool

        if (roleClaim == CommonConstant.Role.Admin || isManager) // Allow ADMIN or ManagerStatus = true
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class AdminOrManagerRequirement : IAuthorizationRequirement { }

using WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace WebApplication.Authorization
{
    public class MarinaManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Marina>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Marina resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // Managers can do anything for the moment.
            if (context.User.IsInRole("Manager"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
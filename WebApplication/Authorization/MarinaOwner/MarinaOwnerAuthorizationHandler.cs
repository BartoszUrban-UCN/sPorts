using WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace WebApplication.Authorization
{
    public class MarinaOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Marina>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Marina resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            var loggedUserId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (loggedUserId == resource.MarinaOwner.PersonId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
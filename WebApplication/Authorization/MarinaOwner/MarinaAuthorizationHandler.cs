using WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebApplication.Authorization;

namespace WebApplication.Authorization.MarinaOwner
{
    public class MarinaAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Marina>
    {
        private readonly UserManager<Person> _userManager;
        public MarinaAuthorizationHandler(UserManager<Person> userManager)
        {
            _userManager = userManager;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Marina resource)
        {
            if (resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.
            if (requirement.Name != Constants.Create &&
                requirement.Name != Constants.Read &&
                requirement.Name != Constants.Update &&
                requirement.Name != Constants.Delete)
            {
                return Task.CompletedTask;
            }

            var loggedUserId = int.Parse(_userManager.GetUserId(context.User));
            
            if (loggedUserId == resource.MarinaOwner.PersonId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
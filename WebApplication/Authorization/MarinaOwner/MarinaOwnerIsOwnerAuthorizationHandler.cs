using WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace WebApplication.Authorization
{
    public class MarinaOwnerIsOwnerAuthorizationHandler
               : AuthorizationHandler<OperationAuthorizationRequirement, MarinaOwner>
    {
        private UserManager<Person> _userManager;

        public MarinaOwnerIsOwnerAuthorizationHandler(UserManager<Person> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, MarinaOwner resource)
        {
            if (context.User == null)
                return Task.CompletedTask;
            
            // Marina Owner can do anything within his scope.
            if (int.Equals(resource.PersonId, _userManager.GetUserId(context.User)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
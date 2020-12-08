using WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;

namespace WebApplication.Authorization.MarinaOwner
{
    public class SpotAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Spot>
    {
        private readonly UserService _userService;

        public SpotAuthorizationHandler(UserService userService)
        {
            _userService = userService;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Spot resource)
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

            // Get the current logged in user's attached Person, and then its related MarinaOwner object
            var loggedPerson = await _userService.GetUserAsync(context.User);
            var marinaOwner = _userService.GetMarinaOwnerFromPerson(loggedPerson);

            if (marinaOwner.MarinaOwnerId == resource.Marina?.MarinaOwnerId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Authorization.BoatOwner
{
    public class BoatAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Boat>
    {
        private readonly UserService _userService;

        public BoatAuthorizationHandler(UserService userService)
        {
            _userService = userService;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Boat resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If user is not a boat owner, return
            if (!context.User.IsInRole(RoleName.BoatOwner))
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD or Booking permission, return.
            if (requirement.Name != Constants.Create &&
                requirement.Name != Constants.Read &&
                requirement.Name != Constants.Update &&
                requirement.Name != Constants.Delete &&
                requirement.Name != Constants.Book)
            {
                return Task.CompletedTask;
            }

            // Get the current logged in user's attached Person, and then its related BoatOwner object
            var loggedPerson = await _userService.GetUserAsync(context.User);
            var boatOwner = _userService.GetBoatOwnerFromPerson(loggedPerson);

            // Verify whether the boat owner that asks for access matches the resource's boat owner information
            if (boatOwner.BoatOwnerId == resource.BoatOwnerId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

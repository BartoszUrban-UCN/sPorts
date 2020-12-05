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
    public class BoatOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Boat>
    {
        private readonly UserService _userService;

        public BoatOwnerAuthorizationHandler(UserService userService)
        {
            _userService = userService;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Boat resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.
            if (requirement.Name != Constants.Create &&
                requirement.Name != Constants.Read &&
                requirement.Name != Constants.Update &&
                requirement.Name != Constants.Delete &&
                !context.User.IsInRole(RoleName.BoatOwner))
            {
                return Task.CompletedTask;
            }

            var loggedPerson = await _userService.GetUserAsync(context.User);
            var boatOwner = _userService.GetBoatOwnerFromPerson(loggedPerson);

            if (boatOwner.BoatOwnerId == resource.BoatOwnerId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

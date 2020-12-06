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
    public class BookingAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Booking>
    {
        private readonly UserService _userService;
        private readonly IBoatService _boatService;

        public BookingAuthorizationHandler(UserService userService, IBoatService boatService)
        {
            _userService = userService;
            _boatService = boatService;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Booking resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for Create or Read permission and user is not a Boat Owner, return.
            if (requirement.Name != Constants.Create &&
                requirement.Name != Constants.Read &&
                !context.User.IsInRole(RoleName.BoatOwner))
            {
                return Task.CompletedTask;
            }

            // Get the current logged in user's attached Person, and then its related BoatOwner object
            var loggedPerson = await _userService.GetUserAsync(context.User);
            var boatOwner = _userService.GetBoatOwnerFromPerson(loggedPerson);

            // Make sure we load all the information we need about the boat in the bookingBoat
            var bookingBoat = await _boatService.GetSingle(resource.BoatId);

            // Verify whether the boat owner that asks for access matches the resource's boat owner information
            if (boatOwner.BoatOwnerId == bookingBoat.BoatOwnerId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;
using System.Linq;
using System.Security.Claims;
using WebApplication.Authorization.BoatOwner;
using WebApplication.Authorization;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BoatController : Controller
    {
        private readonly IBoatService _boatService;
        private readonly IBoatOwnerService _boatOwnerService;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserService _userService;

        public BoatController(IBoatService boatService, IBoatOwnerService boatOwnerService, IAuthorizationService authorizationService, UserService userService)
        {
            _boatService = boatService;
            _boatOwnerService = boatOwnerService;
            _authorizationService = authorizationService;
            _userService = userService;
        }

        // GET: Boat
        public async Task<IActionResult> Index()
        {
            var result = await _boatService.GetAll();

            // User is fully authorized to all content if he is a manager or admin
            var isFullyAuthorized =
                User.IsInRole(RoleName.Administrator) ||
                User.IsInRole(RoleName.Manager);

            // If he is an admin or manager indeed
            if (isFullyAuthorized)
            {
                // Return a view with all the resources displayed
                return View(result);
            }

            // If user is only a boat owner instead
            if (User.IsInRole(RoleName.BoatOwner))
            {
                // Get the logged in user's related boat owner object
                var loggedPerson = await _userService.GetUserAsync(User);
                var boatOwner = _userService.GetBoatOwnerFromPerson(loggedPerson);

                // Filter results so that he only sees his boats rather than all of them
                result = result.Where(boat => boat.BoatOwnerId == boatOwner.BoatOwnerId);

                // Return a view that only displays that boat owner's boats
                return View(result);
            }

            // Forbid access to the page if user is none of the roles of a boat owner, manager or admin
            return Forbid();
        }

        // GET: Boat/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var boat = await _boatService.GetSingle(id);

                // Challenge whether the user is the boat's owner, an admin or a manager
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, boat, Operation.Read);

                // If he owns the access to the boat indeed
                if (isAuthorized.Succeeded)
                    // Return a view with that specific boat
                    return View(boat);

                // If the user shouldn't be able to see that boat's information, forbid the access
                return Forbid();
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        // GET: Boat/Create
        public async Task<IActionResult> Create()
        {
            // User is authorized to create if he is a boat owner, manager or admin
            var isAuthorized =
                User.IsInRole(RoleName.Administrator) ||
                User.IsInRole(RoleName.Manager) ||
                User.IsInRole(RoleName.BoatOwner);

            // If the user is authorized
            if (isAuthorized)
            {
                // Return the Create view
                var boatOwners = await _boatOwnerService.GetAll();
                ViewBag.BoatOwnerId = new SelectList(boatOwners, "BoatOwnerId", "BoatOwnerId");
                return View();
            }

            // If the user is unauthorized for this function, forbid the user's access
            return Forbid();
        }

        // POST: Boat/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventMultipleEvents]
        public async Task<IActionResult> Create([Bind("BoatId,Name,RegistrationNo,Width,Length,Depth,Type,BoatOwnerId")] Boat boat)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check whether user is allowed to create a boat
                    var isAuthorized = await _authorizationService.AuthorizeAsync(User, boat, Operation.Create);

                    // If he is authorized for that
                    if (isAuthorized.Succeeded)
                    {
                        // Create a boat
                        await _boatService.Create(boat);

                        // Persist changes to the database
                        await _boatService.Save();

                        // Return to the Index view of all boats
                        return RedirectToAction(nameof(Index));
                    }

                    // Forbid the post if user is not authorized for that
                    return Forbid();
                }
                catch (BusinessException)
                {
                    return View("Error");
                }
            }

            return View(boat);
        }

        // GET: Boat/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                // Load the boat
                var boat = await _boatService.GetSingle(id);

                // Check whether user is allowed to update the boat
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, boat, Operation.Update);

                // If he is authorized for that
                if (isAuthorized.Succeeded)
                {
                    // Return the Create view
                    var boatOwners = await _boatOwnerService.GetAll();
                    ViewBag.BoatOwnerId = new SelectList(boatOwners, "BoatOwnerId", "BoatOwnerId");
                    return View(boat);
                }

                // Forbid if user is not authorized for that
                return Forbid();
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        // POST: Boat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoatId,Name,RegistrationNo,Width,Length,Depth,Type,BoatOwnerId")] Boat boat)
        {
            if (id != boat.BoatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check whether user is allowed to create a boat
                    var isAuthorized = await _authorizationService.AuthorizeAsync(User, boat, Operation.Update);

                    // If he is authorized for that
                    if (isAuthorized.Succeeded)
                    {
                        // Update the boat
                        _boatService.Update(boat);

                        // Persist changes to the database
                        await _boatService.Save();

                        // Go to index
                        return RedirectToAction(nameof(Index));
                    }

                    // Forbid the post if user is not authorized for that
                    return Forbid();
                }
                catch (BusinessException)
                {
                    return View("Error");
                }
            }

            return View(boat);
        }

        // GET: Boat/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var boat = await _boatService.GetSingle(id);

                // Check whether user is allowed to create a boat
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, boat, Operation.Delete);

                // If he is authorized for that
                if (isAuthorized.Succeeded)
                    // Return the view
                    return View(boat);

                // Forbid the post if user is not authorized for that
                return Forbid();
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        // POST: Boat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                var boat = await _boatService.GetSingle(id);

                // Check whether user is allowed to create a boat
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, boat, Operation.Delete);

                // If he is authorized for that
                if (isAuthorized.Succeeded)
                {
                    // Delete the boat
                    await _boatService.Delete(id);

                    // Persist changes to the database
                    await _boatService.Save();

                    // Go to index
                    return RedirectToAction(nameof(Index));
                }

                // Forbid the post method if the user is unauthorized
                return Forbid();
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }
    }
}

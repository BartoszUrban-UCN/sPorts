using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication.Authorization;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "MarinaOwner")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MarinaController : Controller
    {
        private readonly IMarinaService _marinaService;
        private readonly UserManager<Person> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserService _userService;

        public MarinaController(IMarinaService marinaService, UserManager<Person> userManager,
            IAuthorizationService authorizationService, UserService userService)
        {
            _marinaService = marinaService;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var person = await _userManager.GetUserAsync(User);
                var marinaOwnerId = _userService.GetMarinaOwnerFromPerson(person).MarinaOwnerId;

                var marinas = await _marinaService.GetAll(marinaOwnerId);
                
                return View(marinas);
            }
            catch (BusinessException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var marina = await _marinaService.GetSingle(id);
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Read);
                if (isAuthorized.Succeeded)
                    return View(marina);
                
                return Forbid();
            }
            catch (BusinessException e)
            {
                Console.WriteLine(e);
                throw;
                // Custom 404 page if you wanna be fancy
                //Replace this is a proper return view
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventMultipleEvents]
        public async Task<IActionResult> Create([Bind("Name, Description, Facilities")] Marina marina)
        {
            try
            {
                // Reload the page if the model is invalid
                if (!ModelState.IsValid)
                    return View(marina);
                
                // Set the marina's marinaOwner id to logged user
                var person = await _userManager.GetUserAsync(User);
                var marinaOwnerId = _userService.GetMarinaOwnerFromPerson(person).MarinaOwnerId;
                marina.MarinaOwnerId = marinaOwnerId;

                if (MarinaLocationIsSelected())
                {
                    var marinaLocation = GetLocationFormData();
                    
                    await _marinaService.CreateWithLocation(marina, marinaLocation);
                }
                else
                    await _marinaService.Create(marina);

                await _marinaService.Save();
                // Return to index if successful
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var marinaTask = _marinaService.GetSingle(id);
                var marina = await marinaTask;
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Update);
                if (!isAuthorized.Succeeded)
                    return Forbid();

                var marinaCopy = new Marina
                {
                    Name = marina.Name,
                    Description = marina.Description,
                    Facilities = marina.Facilities,
                    Location = marina.Location,
                    LocationId = marina.LocationId,
                    MarinaOwnerId = marina.MarinaOwnerId
                };

                marinaTask.Dispose();
                
                return View(marinaCopy);
            }
            catch (BusinessException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("MarinaOwnerId, Name, Description, Facilities, LocationId")]
            Marina marina)
        {
            marina.MarinaId = id;
            try
            {
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Update);
                if (!isAuthorized.Succeeded)
                    Forbid();

                if (!ModelState.IsValid)
                    return View(marina);

                if (MarinaLocationIsSelected())
                {
                    var marinaLocation = GetLocationFormData();

                    _marinaService.UpdateMarinaLocation(marina, marinaLocation);
                    await _marinaService.CreateLocationForMarina(marina, marinaLocation);
                }
                else
                    marina = await _marinaService.DeleteMarinaLocation(marina);

                _marinaService.Update(marina);
                await _marinaService.Save();

                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var marina = await _marinaService.GetSingle(id);
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Delete);
                
                if (isAuthorized.Succeeded)
                    return View(marina);
                
                return Forbid();
            }
            catch (BusinessException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PreventMultipleEvents]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var marina = await _marinaService.GetSingle(id);
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Delete);
                if (!isAuthorized.Succeeded)
                    Forbid();
                
                await _marinaService.DeleteMarinaLocation(marina);
                await _marinaService.Delete(id);
                await _marinaService.Save();

                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IActionResult> MarinaSpots(int id)
        {
            try
            {
                var marina = await _marinaService.GetSingle(id);
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Read);
                if (!isAuthorized.Succeeded)
                    Forbid();
                
                ViewData["ViewName"] = "~/Views/Spot/Spot.cshtml";

                var spots = marina.Spots;
                return View("_ListLayout", spots);
            }
            catch (BusinessException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IActionResult> Marina(int id)
        {
            try
            {
                var marina = await _marinaService.GetSingle(id);
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Read);
                if (!isAuthorized.Succeeded)
                    Forbid();

                ViewData["ViewName"] = "Marina";
                var marinaList = new List<Marina> {marina};
                return View("_ListLayout", marinaList);
            }
            catch (BusinessException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool MarinaLocationIsSelected()
        {
            string latitude = Request.Form["Latitude"];
            string longitude = Request.Form["Longitude"];
            string radius = Request.Form["Radius"];

            return latitude.IsNotNullEmptyWhitespace() && longitude.IsNotNullEmptyWhitespace() && radius.IsNotNullEmptyWhitespace();
        }

        private Location GetLocationFormData()
        {
            string latitude = Request.Form["Latitude"];
            string longitude = Request.Form["Longitude"];
            string radius = Request.Form["Radius"];

            if (!latitude.IsNotNullEmptyWhitespace() || !longitude.IsNotNullEmptyWhitespace() ||
                !radius.IsNotNullEmptyWhitespace())
                throw new BusinessException("GetLocationFormData", "Something went wrong when calculating location.");

            Location spotLocation = new Location
            {
                Latitude = Convert.ToDouble(latitude),
                Longitude = Convert.ToDouble(longitude),
                Radius = Convert.ToDouble(radius),
            };

            return spotLocation;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Models;
using Microsoft.AspNetCore.Identity;
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

        public MarinaController(IMarinaService marinaService, UserManager<Person> userManager,
            IAuthorizationService authorizationService)
        {
            _marinaService = marinaService;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = int.Parse(_userManager.GetUserId(User));
                
                var marinas = await _marinaService.GetAll(userId);
                
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
                id.ThrowIfInvalidId();

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
            //ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventMultipleEvents]
        public async Task<IActionResult> Create([Bind("Name,Description,Facilities")] Marina marina)
        {
            if (!ModelState.IsValid)
            { 
                //ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId",
                                                //"AddressId", marina.AddressId); 
                return View(marina);
            }

            var loggedUserId = int.Parse(_userManager.GetUserId(User));
            marina.MarinaOwnerId = loggedUserId;
                
            // Marina with Location on map
            if (MarinaLocationIsSelected())
            {
                var marinaLocation = GetLocationFormData();

                if (marinaLocation is not null)
                    await _marinaService.CreateWithLocation(marina, marinaLocation);
            }
            // Marina without Location on map
            else
                await _marinaService.Create(marina);

            try
            {
                await _marinaService.Save();
            }
            catch (BusinessException e)
            {
                Console.WriteLine(e);
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var marina = await _marinaService.GetSingle(id);
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Update);

                //ViewData["AddressId"] = new SelectList(_context.Addresses,"AddressId",
                                            //"AddressId", marina.AddressId);
                
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,
            [Bind("Name,Description,Facilities,LocationId")]
            Marina marina)
        {
            if (!ModelState.IsValid)
            {
                //ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId",
                                                                 //"AddressId", marina.AddressId);
                return View(marina);
            }
            
            var loggedUserId = int.Parse(_userManager.GetUserId(User));
            marina.MarinaOwner = new MarinaOwner { PersonId = loggedUserId };
            
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Update);

            if (!isAuthorized.Succeeded)
                Forbid();

            if (MarinaLocationIsSelected())
            {
                var marinaLocation = GetLocationFormData();

                if (marinaLocation is not null)
                {
                    if (marina.LocationId.IsValidId())
                        _marinaService.UpdateMarinaLocation(marina, marinaLocation);
                    else
                        await _marinaService.CreateLocationForMarina(marina, marinaLocation);
                }
            }
            else
            {
                marina.LocationId.ThrowIfInvalidId(); 
                marina = await _marinaService.DeleteMarinaLocation(marina);
            }

            _marinaService.Update(marina);
            await _marinaService.Save();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                id.ThrowIfInvalidId();

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
                id.ThrowIfNegativeId();
                
                var marina = await _marinaService.GetSingle(id);
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Delete);
                if (!isAuthorized.Succeeded)
                    Forbid();
                
                marina.LocationId.ThrowIfInvalidId();

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
                var marinaList = new List<Marina>();
                marinaList.Add(marina);
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

            if (latitude.IsNotNullEmptyWhitespace() && longitude.IsNotNullEmptyWhitespace() && radius.IsNotNullEmptyWhitespace())
                return true;

            return false;
        }

        private Location GetLocationFormData()
        {
            string latitude = Request.Form["Latitude"];
            string longitude = Request.Form["Longitude"];
            string radius = Request.Form["Radius"];

            if (latitude.IsNotNullEmptyWhitespace() && longitude.IsNotNullEmptyWhitespace() && radius.IsNotNullEmptyWhitespace())
            {
                Location spotLocation = new Location
                {
                    Latitude = Convert.ToDouble(latitude),
                    Longitude = Convert.ToDouble(longitude),
                    Radius = Convert.ToDouble(radius),
                };

                return spotLocation;
            }

            return null;
        }
    }
}

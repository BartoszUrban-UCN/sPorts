﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication.Authorization;

namespace WebApplication.Controllers
{
    [Authorize(Roles="MarinaOwner")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SpotController : Controller
    {
        private readonly ISpotService _spotService;
        private readonly IMarinaService _marinaService;
        private readonly UserManager<Person> _userManager;
        private readonly UserService _userService;
        private readonly IAuthorizationService _authorizationService;
        
        public SpotController(ISpotService spotService, UserService userService,
            IMarinaService marinaService, UserManager<Person> userManager,
            IAuthorizationService authorizationService)
        {
            _spotService = spotService;
            _marinaService = marinaService;
            _userManager = userManager;
            _userService = userService;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var person = await _userManager.GetUserAsync(User);
                var marinaOwnerId = _userService.GetMarinaOwnerFromPerson(person).MarinaOwnerId; 
                
                var spots = await _spotService.GetAll(marinaOwnerId);
                
                return View(spots);
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
                var spot = await _spotService.GetSingle(id);

                var isAuthorized = await _authorizationService.AuthorizeAsync(User, spot, Operation.Read);
                if (isAuthorized.Succeeded)
                    return View(spot);

                return Forbid();
            }
            catch (BusinessException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<SelectList> MarinaList()
        {
            var person = await _userManager.GetUserAsync(User);
            var marinaOwnerId = _userService.GetMarinaOwnerFromPerson(person).MarinaOwnerId;

            var marinas = await _marinaService.GetAll(marinaOwnerId);
            var marinaList = new SelectList(marinas, "MarinaId", "MarinaId");
            
            return marinaList;
        }
        
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewData["MarinaId"] = await MarinaList();
                
                return View();
            }
            catch (BusinessException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventMultipleEvents]
        public async Task<IActionResult> Create([Bind("SpotNumber,Available,MaxWidth,MaxLength,MaxDepth,Price,MarinaId")] Spot spot)
        {
            
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["MarinaId"] = await MarinaList();
                    
                    return View(spot);
                }
                
                // If user has chosen a location for the spot by using the Leaflet map
                if (SpotLocationIsSelected())
                {
                    // Create related data (Location) for the Spot and assign the newly created Location to the Spot
                    var location = GetLocationData();
                    await _spotService.CreateWithLocation(spot, location);
                }
                else
                    await _spotService.Create(spot);

                await _spotService.Save();
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException exception)
            {
                ModelState.TryAddModelError(exception.Key, exception.Message);
                throw;
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var spotTask = _spotService.GetSingle(id);
                var spot = await spotTask;
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, spot, Operation.Update);
                if (!isAuthorized.Succeeded)
                    Forbid();

                var spotCopy = new Spot
                {
                    SpotNumber = spot.SpotNumber,
                    Available = spot.Available,
                    MaxWidth = spot.MaxWidth,
                    MaxLength = spot.MaxLength,
                    MaxDepth = spot.MaxDepth,
                    Price = spot.Price,
                    MarinaId = spot.MarinaId,
                    LocationId = spot.LocationId,
                };
                
                ViewData["MarinaId"] = await MarinaList(); 
                
                spotTask.Dispose();
                
                return View(spotCopy);
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
            [Bind("SpotNumber,Available,MaxWidth,MaxLength,MaxDepth,Price,MarinaId,LocationId")]
            Spot spot)
        {
            try
            {
                var spotDb = await _spotService.GetSingle(id);
                var locationDb = spotDb.Location;
                
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, spotDb, Operation.Update);
                if (!isAuthorized.Succeeded)
                    Forbid();

                if (!ModelState.IsValid)
                {
                    ViewData["MarinaId"] = await MarinaList();

                    return View(spot);
                }

                spotDb.SpotNumber = spot.SpotNumber;
                spotDb.Available = spot.Available;
                spotDb.MaxWidth = spot.MaxWidth;
                spotDb.MaxLength = spot.MaxLength;
                spotDb.MaxDepth = spot.MaxDepth;
                spotDb.Price = spot.Price;
                spotDb.MarinaId = spot.MarinaId;
                spotDb.LocationId = spot.LocationId;
                
                // If user has chosen a location for the spot by using the Leaflet map
                if (SpotLocationIsSelected())
                {
                    // Either update the location linked to the spot with the new data
                    // Or create related data (Location) for the Spot and assign the newly created Location to the Spot
                    var location = GetLocationData();
                    locationDb.Latitude = location.Latitude;
                    locationDb.Longitude = location.Longitude;
                    await _spotService.UpdateSpotLocation(spotDb, locationDb);
                }
                // But if the spot does not have a location now
                else
                {
                    // And if the spot has had a location assigned to it before but now the user removed it
                    if (spotDb.LocationId != null)
                        // Delete the spot's location
                        spotDb = await _spotService.DeleteSpotLocation(spotDb);
                }

                _spotService.Update(spotDb);
                await _spotService.Save();

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
                var spot = await _spotService.GetSingle(id);

                var isAuthorized = await _authorizationService.AuthorizeAsync(User, spot, Operation.Delete);
                if (!isAuthorized.Succeeded)
                    return View(spot);

                return Forbid();
            }
            catch (BusinessException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var spot = await _spotService.GetSingle(id);

                var isAuthorized = await _authorizationService.AuthorizeAsync(User, spot, Operation.Delete);
                if (!isAuthorized.Succeeded)
                    Forbid();
                
                await _spotService.Delete(id);
                await _spotService.Save();
                
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async Task<bool> SpotExists(int id)
        {
            return await _spotService.Exists(id);
        }

        public bool SpotLocationIsSelected()
        {
            String Latitude = Request.Form["Latitude"];
            String Longitude = Request.Form["Longitude"];

            return !String.IsNullOrEmpty(Latitude) && !String.IsNullOrEmpty(Longitude);
        }

        private Location GetLocationData()
        {
            string Latitude = Request.Form["Latitude"];
            string Longitude = Request.Form["Longitude"];

            Location spotLocation = new Location
            {
                Latitude = Convert.ToDouble(Latitude, CultureInfo.InvariantCulture),
                Longitude = Convert.ToDouble(Longitude, CultureInfo.InvariantCulture)
            };

            return spotLocation;
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SpotController : Controller
    {
        private readonly ISpotService _spotService;
        private readonly IMarinaService _marinaService;

        public SpotController(ISpotService spotService, IMarinaService marinaService)
        {
            _spotService = spotService;
            _marinaService = marinaService;
        }

        // GET: Spot
        public async Task<IActionResult> Index()
        {
            return View(await _spotService.GetAll());
        }

        // GET: Spot/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spot = await _spotService.GetSingle(id);

            if (spot == null)
            {
                return NotFound();
            }

            return View(spot);
        }

        // GET: Spot/Create
        public async Task<IActionResult> Create()
        {
            ViewData["MarinaId"] = new SelectList(await _marinaService.GetAll(), "MarinaId", "MarinaId");
            return View();
        }

        // POST: Spot/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SpotId,SpotNumber,Available,MaxWidth,MaxLength,MaxDepth,Price,MarinaId")] Spot spot)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // If user has chosen a location for the spot by using the Leaflet map
                    if (SpotLocationIsSelected())
                    {
                        // Create related data (Location) for the Spot and assign the newly created Location to the Spot
                        var location = GetLocationData();
                        await _spotService.CreateWithLocation(spot, location);
                    }
                    else
                    {
                        await _spotService.Create(spot);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessException exception)
                {
                    ModelState.TryAddModelError(exception.Key, exception.Message);
                }
            }

            ViewData["MarinaId"] = new SelectList(await _marinaService.GetAll(), "MarinaId", "MarinaId", spot.MarinaId);
            return View(spot);
        }

        // GET: Spot/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spot = await _spotService.GetSingle(id);

            if (spot == null)
            {
                return NotFound();
            }

            ViewData["MarinaId"] = new SelectList(await _marinaService.GetAll(), "MarinaId", "MarinaId", spot.MarinaId);
            return View(spot);
        }

        // POST: Spot/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SpotId,SpotNumber,Available,MaxWidth,MaxLength,MaxDepth,Price,MarinaId,LocationId")] Spot spot)
        {
            if (id != spot.SpotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // If user has chosen a location for the spot by using the Leaflet map
                    if (SpotLocationIsSelected())
                    {
                        // Either update the location linked to the spot with the new data
                        // Or create related data (Location) for the Spot and assign the newly created Location to the Spot
                        var location = GetLocationData();
                        await _spotService.UpdateSpotLocation(spot, location);
                    }
                    // But if the spot does not have a location now
                    else
                    {
                        // And if the spot has had a location assigned to it before but now the user removed it
                        if (spot.LocationId != null)
                        {
                            // Delete the spot's location
                            spot = await _spotService.DeleteSpotLocation(spot);
                        }
                    }

                    _spotService.Update(spot);
                    await _spotService.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await SpotExists(spot.SpotId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MarinaId"] = new SelectList(await _marinaService.GetAll(), "MarinaId", "MarinaId", spot.MarinaId);
            return View(spot);
        }

        // GET: Spot/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spot = await _spotService.GetSingle(id);

            if (spot == null)
            {
                return NotFound();
            }

            return View(spot);
        }

        // POST: Spot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // TODO: This deletion type could be replaced by a delete on cascade (Spot -> Location)
            await _spotService.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SpotExists(int id)
        {
            return await _spotService.Exists(id);
        }

        public bool SpotLocationIsSelected()
        {
            String Latitude = Request.Form["Latitude"];
            String Longitude = Request.Form["Longitude"];

            if (String.IsNullOrEmpty(Latitude) || String.IsNullOrEmpty(Longitude))
            {
                return false;
            }

            return true;
        }

        private Location GetLocationData()
        {
            string Latitude = Request.Form["Latitude"];
            string Longitude = Request.Form["Longitude"];

            Location spotLocation = new Location
            {
                Latitude = Convert.ToDouble(Latitude),
                Longitude = Convert.ToDouble(Longitude)
            };

            return spotLocation;
        }
    }
}

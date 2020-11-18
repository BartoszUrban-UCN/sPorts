﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MarinaController : Controller
    {
        private readonly SportsContext _context;

        public MarinaController(SportsContext context)
        {
            _context = context;
        }

        // GET: Marina
        public async Task<IActionResult> Index()
        {
            // TODO: Including all the spots might be a bit heavyweight on the system, but let's see if the delay is acceptable P.S. Need it for marina location generation
            var sportsContext = _context.Marinas
                .Include(m => m.Address)
                .Include(m => m.MarinaOwner)
                .Include(m => m.Location)
                .Include(m => m.Spots)
                    .ThenInclude(s => s.Location);

            foreach (Marina marina in sportsContext.ToList())
            {
                if (marina.Location == null)
                {
                    if (MarinaHasSpotsLocations(marina))
                    {
                        await CalculateMarinaLocation(marina);
                    }
                }
            }

            return View(await sportsContext.ToListAsync());
        }

        // GET: Marina/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marina = await _context.Marinas
                .Include(m => m.Address)
                .Include(m => m.MarinaOwner)
                .FirstOrDefaultAsync(m => m.MarinaId == id);

            if (marina == null)
            {
                return NotFound();
            }

            return View(marina);
        }

        // GET: Marina/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId");
            ViewData["MarinaOwnerId"] = new SelectList(_context.MarinaOwners, "MarinaOwnerId", "MarinaOwnerId");
            return View();
        }

        // POST: Marina/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MarinaId,Name,Description,Facilities,MarinaOwnerId,AddressId")] Marina marina)
        {
            if (ModelState.IsValid)
            {
                _context.Add(marina);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", marina.AddressId);
            ViewData["MarinaOwnerId"] = new SelectList(_context.MarinaOwners, "MarinaOwnerId", "MarinaOwnerId", marina.MarinaOwnerId);
            return View(marina);
        }

        // GET: Marina/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marina = await _context.Marinas.FindAsync(id);
            if (marina == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", marina.AddressId);
            ViewData["MarinaOwnerId"] = new SelectList(_context.MarinaOwners, "MarinaOwnerId", "MarinaOwnerId", marina.MarinaOwnerId);
            return View(marina);
        }

        // POST: Marina/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MarinaId,Name,Description,Facilities,MarinaOwnerId,AddressId")] Marina marina)
        {
            if (id != marina.MarinaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marina);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarinaExists(marina.MarinaId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", marina.AddressId);
            ViewData["MarinaOwnerId"] = new SelectList(_context.MarinaOwners, "MarinaOwnerId", "MarinaOwnerId", marina.MarinaOwnerId);
            return View(marina);
        }

        // GET: Marina/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marina = await _context.Marinas
                .Include(m => m.Address)
                .Include(m => m.MarinaOwner)
                .FirstOrDefaultAsync(m => m.MarinaId == id);
            if (marina == null)
            {
                return NotFound();
            }

            return View(marina);
        }

        // POST: Marina/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marina = await _context.Marinas.FindAsync(id);
            _context.Marinas.Remove(marina);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarinaExists(int id)
        {
            return _context.Marinas.Any(e => e.MarinaId == id);
        }

        public async Task<IActionResult> MarinaSpots(int id)
        {
            var marinaWithSpots = await _context.Marinas.Include(s => s.Spots).ToListAsync();
            var marina = marinaWithSpots.Find(m => m.MarinaId == id);

            ViewData["ViewName"] = "~/Views/Spot/Spot.cshtml";
            if (marina != null)
            {
                var spots = marina.Spots;
                return View("_ListLayout", spots);
            }
            return View("Error");
        }

        public async Task<IActionResult> Marina(int id)
        {
            ViewData["ViewName"] = "Marina";

            var marina = await _context.Marinas.FindAsync(id);

            if (marina != null)
            {
                var marinaList = new List<Marina>();
                marinaList.Add(marina);
                return View("_ListLayout", marinaList);
            }
            return View("Error");
        }

        public async Task<IActionResult> CreateAssignLocationToMarina(Marina marina)
        {
            string XLatitude = Request.Form["XLatitude"];
            string YLongitude = Request.Form["YLongitude"];

            Location spotLocation = new Location
            {
                XLatitude = Convert.ToDouble(XLatitude),
                YLongitude = Convert.ToDouble(YLongitude)
            };

            var locationController = new LocationController(_context);
            IActionResult result = await locationController.Create(spotLocation);

            marina.LocationId = spotLocation.LocationId;

            return result;
        }

        public bool MarinaHasSpotsLocations(Marina marina)
        {
            foreach (Spot spot in marina.Spots)
            {
                if (spot.LocationId != null)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<IActionResult> CalculateMarinaLocation(Marina marina)
        {
            double latitudeSum = 0;
            double longitudeSum = 0;
            double locationsTotal = 0;

            foreach (Spot spot in marina.Spots)
            {
                if (spot.LocationId != null)
                {
                    locationsTotal += 1;
                    latitudeSum += spot.Location.XLatitude;
                    longitudeSum += spot.Location.YLongitude;
                }
            }

            Location marinaLocation = new Location
            {
                XLatitude = latitudeSum / locationsTotal,
                YLongitude = longitudeSum / locationsTotal,
            };

            var locationController = new LocationController(_context);
            IActionResult result = await locationController.Create(marinaLocation);

            marina.LocationId = marinaLocation.LocationId;
            marina.Location = marinaLocation;

            return result;
        }
    }
}

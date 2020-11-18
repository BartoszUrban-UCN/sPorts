using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SpotController : Controller
    {
        private readonly SportsContext _context;

        public SpotController(SportsContext context)
        {
            _context = context;
        }

        // GET: Spot
        public async Task<IActionResult> Index()
        {
            var sportsContext = _context.Spots
                .Include(spot => spot.Marina);

            // TODO: The loading of the Locations could be replaced by a ViewBag, just like Marina uses (see line 87 for example)
            foreach (Spot spot in sportsContext)
            {
                _context.Locations.Where(location => location.LocationId == spot.LocationId).Load();
            }

            return View(await sportsContext.ToListAsync());
        }

        // GET: Spot/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spot = await _context.Spots
                .Include(s => s.Marina)
                .FirstOrDefaultAsync(m => m.SpotId == id);

            // Location related to the spot is loaded
            // TODO: Could have probably been done by using ThenInclude directly in the spot
            _context.Locations.Where(location => location.LocationId == spot.LocationId).Load();

            if (spot == null)
            {
                return NotFound();
            }

            return View(spot);
        }

        // GET: Spot/Create
        public IActionResult Create()
        {
            ViewData["MarinaId"] = new SelectList(_context.Marinas, "MarinaId", "MarinaId");

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
                // If user has chosen a location for the spot by using the Leaflet map
                if (SpotLocationIsSelected())
                {
                    // Create related data (Location) for the Spot and assign the newly created Location to the Spot
                    await CreateAssignLocationToSpot(spot);
                }

                _context.Add(spot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MarinaId"] = new SelectList(_context.Marinas, "MarinaId", "MarinaId", spot.MarinaId);

            return View(spot);
        }

        // GET: Spot/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spot = await _context.Spots.FindAsync(id);

            if (spot == null)
            {
                return NotFound();
            }

            _context.Locations.Where(location => location.LocationId == spot.LocationId).Load();
            ViewData["MarinaId"] = new SelectList(_context.Marinas, "MarinaId", "MarinaId", spot.MarinaId);
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
                        if (spot.LocationId != null)
                        {
                            await UpdateSpotLocation(spot);
                        }
                        // Or create related data (Location) for the Spot and assign the newly created Location to the Spot
                        else
                        {
                            await CreateAssignLocationToSpot(spot);
                        }
                    }
                    // But if the spot does not have a location now
                    else
                    {
                        // And if the spot has had a location assigned to it before but now the user removed it
                        if (spot.LocationId != null)
                        {
                            // Delete the spot's location
                            await DeleteSpotLocation(spot);
                        }
                    }

                    _context.Update(spot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpotExists(spot.SpotId))
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

            ViewData["MarinaId"] = new SelectList(_context.Marinas, "MarinaId", "MarinaId", spot.MarinaId);
            return View(spot);
        }

        // GET: Spot/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spot = await _context.Spots
                .Include(s => s.Marina)
                .FirstOrDefaultAsync(m => m.SpotId == id);

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
            var spot = await _context.Spots.FindAsync(id);

            // TODO: This deletion type could be replaced by a delete on cascade (Spot -> Location)
            await DeleteSpotLocation(spot);

            _context.Spots.Remove(spot);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool SpotExists(int id)
        {
            return _context.Spots.Any(e => e.SpotId == id);
        }

        [Route("spot/{id}")]
        public async Task<IActionResult> Spot(int id)
        {
            ViewData["ViewName"] = "Spot";

            var spot = _context.Spots.FindAsync(id);
            var spotList = new List<Spot>();
            spotList.Add(await spot);

            return View("_ListLayout", spotList);
        }

        public async Task<IActionResult> Marina(int id)
        {
            var spotsWithMarina = await _context.Spots.Include(s => s.Marina).ToListAsync();
            var spot = spotsWithMarina.Find(spot => spot.SpotId == id);

            if (spot != null)
            {
                return View("~/Views/Marina/Marina.cshtml", spot.Marina);
            }
            return View("Error");
        }

        public bool SpotLocationIsSelected()
        {
            String XLatitude = Request.Form["XLatitude"];
            String YLongitude = Request.Form["YLongitude"];

            if (String.IsNullOrEmpty(XLatitude) || String.IsNullOrEmpty(YLongitude))
            {
                return false;
            }

            return true;
        }

        public async Task<IActionResult> CreateAssignLocationToSpot(Spot spot)
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

            spot.LocationId = spotLocation.LocationId;

            return result;
        }

        public async Task<IActionResult> UpdateSpotLocation(Spot spot)
        {
            string XLatitude = Request.Form["XLatitude"];
            string YLongitude = Request.Form["YLongitude"];

            Location spotLocation = _context.Locations.Find(spot.LocationId);

            spotLocation.XLatitude = Convert.ToDouble(XLatitude);
            spotLocation.YLongitude = Convert.ToDouble(YLongitude);

            var locationController = new LocationController(_context);
            IActionResult result = await locationController.Edit(spotLocation.LocationId, spotLocation);

            return result;
        }

        public async Task<IActionResult> DeleteSpotLocation(Spot spot)
        {
            Location spotLocation = _context.Locations.Find(spot.LocationId);

            var locationController = new LocationController(_context);
            IActionResult result = await locationController.Delete(spot.LocationId);

            spot.LocationId = null;

            return result;
        }
    }
}

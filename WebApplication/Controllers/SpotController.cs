using System;
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
            var sportsContext = _context.Spots.Include(s => s.Marina);
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
            ViewData["MarinaId"] = new SelectList(_context.Marinas, "MarinaId", "MarinaId", spot.MarinaId);
            return View(spot);
        }

        // POST: Spot/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SpotId,SpotNumber,Available,MaxWidth,MaxLength,MaxDepth,Price,MarinaId")] Spot spot)
        {
            if (id != spot.SpotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            _context.Spots.Remove(spot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpotExists(int id)
        {
            return _context.Spots.Any(e => e.SpotId == id);
        }

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
    }
}

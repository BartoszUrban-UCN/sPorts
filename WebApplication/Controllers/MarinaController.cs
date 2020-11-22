using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.BusinessLogic.Shared;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MarinaController : Controller
    {
        private readonly SportsContext _context;
        private readonly IMarinaService _marinaService;

        public MarinaController(SportsContext context, IMarinaService marinaService)
        {
            _context = context;
            _marinaService = marinaService;
        }

        // GET: Marina
        public async Task<IActionResult> Index()
        {
            return View(await _marinaService.GetAll());
        }

        // GET: Marina/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id.IsValidId())
                if (await _marinaService.Exists(id))
                {
                    var marina = await _marinaService.GetSingle(id);

                    if (marina is not null)
                        return View(marina);
                }

            return NotFound();
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
                if (MarinaLocationIsSelected())
                {
                    var marinaLocation = GetLocationFormData();

                    if (marinaLocation is not null)
                        await _marinaService.CreateWithLocation(marina, marinaLocation);
                }
                else
                    await _marinaService.Create(marina);

                return RedirectToAction(nameof(Index));
            }

            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", marina.AddressId);
            ViewData["MarinaOwnerId"] = new SelectList(_context.MarinaOwners, "MarinaOwnerId", "MarinaOwnerId", marina.MarinaOwnerId);
            return View(marina);
        }

        // GET: Marina/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id.IsNotValidId() || await _marinaService.NotExists(id))
                return NotFound();

            var marina = await _context.Marinas.Include(m => m.Spots).FirstOrDefaultAsync(m => m.MarinaId == id);

            if (marina == null)
                return NotFound();

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
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marina);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _marinaService.Exists(marina.MarinaId))
                        return NotFound();
                    else
                        throw;
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
            if (id.IsValidId())
                if (await _marinaService.Exists(id))
                {
                    var marina = await _marinaService.GetSingle(id);

                    if (marina is not null)
                        return View(marina);
                }

            return NotFound();
        }

        // POST: Marina/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _marinaService.Exists(id))
            {
                var marina = await _marinaService.GetSingle(id);

                if (marina.LocationId is not null)
                    await _marinaService.DeleteMarinaLocation(marina);

                // The delete method in the service checks whether the id is: null, negative, not assigned to any marina
                await _marinaService.Delete(id);
            }    

            return RedirectToAction(nameof(Index));
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

        public bool MarinaLocationIsSelected()
        {
            string latitude = Request.Form["Latitude"];
            string longitude = Request.Form["Longitude"];
            string radius = Request.Form["Radius"];

            if (latitude.IsNotNullOrEmpty() && longitude.IsNotNullOrEmpty() && radius.IsNotNullOrEmpty())
                return false;

            return true;
        }

        private Location GetLocationFormData()
        {
            string latitude = Request.Form["Latitude"];
            string longitude = Request.Form["Longitude"];
            string radius = Request.Form["Radius"];

            if (latitude.IsNotNullOrEmpty() && longitude.IsNotNullOrEmpty() && radius.IsNotNullOrEmpty())
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

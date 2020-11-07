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
            var sportsContext = _context.Marinas.Include(m => m.Address).Include(m => m.MarinaOwner);
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
    }
}

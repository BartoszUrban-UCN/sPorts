using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class MarinaOwnerController : Controller
    {
        private readonly SportsContext _context;

        public MarinaOwnerController(SportsContext context)
        {
            _context = context;
        }

        // GET: MarinaOwner
        public async Task<IActionResult> Index()
        {
            var sportsContext = _context.MarinaOwners.Include(m => m.Person);
            return View(await sportsContext.ToListAsync());
        }

        // GET: MarinaOwner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marinaOwner = await _context.MarinaOwners
                .Include(m => m.Person)
                .Include(m=> m.Marina)
                .FirstOrDefaultAsync(m => m.MarinaOwnerId == id);
            if (marinaOwner == null)
            {
                return NotFound();
            }

            return View(marinaOwner);
        }

        // GET: MarinaOwner/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.Persons, "PersonId", "Email");
            return View();
        }

        // POST: MarinaOwner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MarinaOwnerId,PersonId")] MarinaOwner marinaOwner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(marinaOwner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.Persons, "PersonId", "Email", marinaOwner.PersonId);
            return View(marinaOwner);
        }

        // GET: MarinaOwner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marinaOwner = await _context.MarinaOwners.Include(m => m.Marina).FirstOrDefaultAsync(m => m.MarinaOwnerId == id);
            if (marinaOwner == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.Persons, "PersonId", "Email", marinaOwner.PersonId);
            return View(marinaOwner);
        }

        // POST: MarinaOwner/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MarinaOwnerId,PersonId")] MarinaOwner marinaOwner)
        {
            if (id != marinaOwner.MarinaOwnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marinaOwner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarinaOwnerExists(marinaOwner.MarinaOwnerId))
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
            ViewData["PersonId"] = new SelectList(_context.Persons, "PersonId", "Email", marinaOwner.PersonId);
            return View(marinaOwner);
        }

        // GET: MarinaOwner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marinaOwner = await _context.MarinaOwners
                .Include(m => m.Person)
                .FirstOrDefaultAsync(m => m.MarinaOwnerId == id);
            if (marinaOwner == null)
            {
                return NotFound();
            }

            return View(marinaOwner);
        }

        // POST: MarinaOwner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marinaOwner = await _context.MarinaOwners.FindAsync(id);
            _context.MarinaOwners.Remove(marinaOwner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarinaOwnerExists(int id)
        {
            return _context.MarinaOwners.Any(e => e.MarinaOwnerId == id);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PersonController : Controller
    {
        private readonly SportsContext _context;
        private readonly ILoginService _loginService;

        public PersonController(SportsContext context, ILoginService loginService)
        {
            _context = context;
            _loginService = loginService;
        }

        // GET: Person
        public async Task<IActionResult> Index()
        {
            var sportsContext = _context.Persons.Include(p => p.Address);
            return View(await sportsContext.ToListAsync());
        }

        // GET: Person/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .Include(p => p.Address)
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: Person/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId");
            return View();
        }

        // POST: Person/Create To protect from overposting attacks, enable the specific properties
        // you want to bind to, for more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,AddressId,Email,FirstName,LastName,Password,PhoneNumber")] Person person)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _loginService.Create(person);
                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessException exception)
                {
                    ModelState.TryAddModelError(exception.Key, exception.Message);
                }
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", person.AddressId);
            return View(person);
        }

        // GET: Person/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", person.AddressId);
            return View(person);
        }

        // POST: Person/Edit/5 To protect from overposting attacks, enable the specific properties
        // you want to bind to, for more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonId,AddressId,Email,FirstName,LastName,Password,PhoneNumber")] Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", person.AddressId);
            return View(person);
        }

        // GET: Person/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .Include(p => p.Address)
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Person/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.PersonId == id);
        }
    }
}

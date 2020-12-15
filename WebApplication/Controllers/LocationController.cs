using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "MarinaOwner")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LocationController : Controller
    {
        private readonly ILocationService _service;

        public LocationController(ILocationService service)
        {
            _service = service;
        }

        // GET: Location
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAll());
        }

        // GET: Location/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _service.GetSingle(id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // GET: Location/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationId,Latitude,Longitude")] Location location)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.Create(location);
                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessException exception)
                {
                    ModelState.TryAddModelError(exception.Key, exception.Message);
                }
            }
            return View(location);
        }

        // GET: Location/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _service.GetSingle(id);

            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Location/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationId,Latitude,Longitude")] Location location)
        {
            if (id != location.LocationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _service.Update(location);
                }
                catch (BusinessException ex)
                {
                    if (!await LocationExists(location.LocationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.TryAddModelError(ex.Key, ex.Message);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Location/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _service.GetSingle(id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> LocationExists(int id)
        {
            return await _service.Exists(id);
        }
    }
}

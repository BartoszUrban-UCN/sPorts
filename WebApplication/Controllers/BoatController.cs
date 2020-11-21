using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using System.Collections.Generic;
using System.Linq;
using WebApplication.BusinessLogic;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BoatController : Controller
    {
        private readonly IBoatService _boatService;
        private readonly IBoatOwnerService _boatOwnerService;
        public BoatController(IBoatService boatService, IBoatOwnerService boatOwnerService)
        {
            _boatService = boatService;
            _boatOwnerService = boatOwnerService;
        }
        // GET: Boats
        public async Task<IActionResult> Index()
        {
            var result = await _boatService.GetAll();
            return View(result);
        }

        // GET: Boats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var boat = await _boatService.GetSingle(id);
                return View(boat);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        // GET: Boats/Create
        public IActionResult Create()
        {
            var boatOwners = _boatOwnerService.GetAll();
            var boatOwnerId = ViewData["BoatOwnerId"];
            return View();
        }

        // POST: Boats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatId,Name,RegistrationNo,Width,Length,Depth,Type,BoatOwnerId")] Boat boat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _boatService.Create(boat);
                    return RedirectToAction(nameof(Index));
                }

                var boatOwnerId = ViewData["BoatOwnerId"];
                return View(boat);
            }
            catch (BusinessException)
            { }
            return View("Error");
        }

        // GET: Boats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var boat = await _boatService.GetSingle(id);
                var boatOwnerId = ViewData["BoatOwnerId"];
                return View(boat);
            }
            catch (BusinessException)
            { }

            return View("Error");
        }

        // POST: Boats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoatId,Name,RegistrationNo,Width,Length,Depth,Type,BoatOwnerId")] Boat boat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _boatService.Update(boat);
                    var boatOwnerId = ViewData["BoatOwnerId"];
                    return View(boat);
                }
            }
            catch (BusinessException)
            { }

            return View("Error");
        }

        // GET: Boats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var boat = await _boatService.GetSingle(id);
                return View(boat);
            }
            catch (BusinessException)
            { }

            return View("Error");
        }

        // POST: Boats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                await _boatService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException)
            { }

            return View("Error");
        }

        private async Task<bool> BoatExists(int? id)
        {
            return await _boatService.Exists(id);
        }
    }
}

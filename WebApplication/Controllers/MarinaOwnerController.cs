using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Authorization;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "Manager")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MarinaOwnerController : Controller
    {
        private readonly IMarinaOwnerService _marinaOwnerService;
        private readonly IBookingLineService _bookingLineService;
        private readonly UserService _userService;

        public MarinaOwnerController(IMarinaOwnerService marinaOwnerService, IBookingLineService bookingLineService, UserService userService)
        {
            _marinaOwnerService = marinaOwnerService;
            _bookingLineService = bookingLineService;
            _userService = userService;
        }

        // GET: MarinaOwner
        public async Task<IActionResult> Index()
        {
            var marinaOwners = await _marinaOwnerService.GetAll();
            return View(marinaOwners);
        }

        // GET: MarinaOwner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marinaOwner = await _marinaOwnerService.GetSingle(id);
            if (marinaOwner == null)
            {
                return NotFound();
            }

            return View(marinaOwner);
        }

        // GET: MarinaOwner/Create
        public async Task<IActionResult> Create()
        {
            // should get all people, personService??
            var people = await _marinaOwnerService.GetAll();
            ViewData["PersonId"] = new SelectList(people, "PersonId", "Email");
            return View();
        }

        // POST: MarinaOwner/Create To protect from overposting attacks, enable
        // the specific properties you want to bind to. For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventMultipleEvents]
        public async Task<IActionResult> Create([Bind("MarinaOwnerId,PersonId")] MarinaOwner marinaOwner)
        {
            if (ModelState.IsValid)
            {
                await _marinaOwnerService.Create(marinaOwner);
                return RedirectToAction(nameof(Index));
            }

            // should get all people, personService??
            var people = await _marinaOwnerService.GetAll();
            ViewData["PersonId"] = new SelectList(people, "PersonId", "Email", marinaOwner.PersonId);
            return View(marinaOwner);
        }

        // GET: MarinaOwner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marinaOwner = await _marinaOwnerService.GetSingle(id);
            if (marinaOwner == null)
            {
                return NotFound();
            }

            // should get all people, personService??
            var people = await _marinaOwnerService.GetAll();
            ViewData["PersonId"] = new SelectList(people, "PersonId", "Email", marinaOwner.PersonId);
            return View(marinaOwner);
        }

        // POST: MarinaOwner/Edit/5 To protect from overposting attacks, enable
        // the specific properties you want to bind to. For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    _marinaOwnerService.Update(marinaOwner);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _marinaOwnerService.Exists(marinaOwner.MarinaOwnerId))
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

            // should get all people, personService??
            var people = await _marinaOwnerService.GetAll();
            ViewData["PersonId"] = new SelectList(people, "PersonId", "Email", marinaOwner.PersonId);
            return View(marinaOwner);
        }

        // GET: MarinaOwner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marinaOwner = await _marinaOwnerService.GetSingle(id);
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
            await _marinaOwnerService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Get Bookings of logged in Marina Owner
        /// </summary>
        /// <returns>View</returns>
        // GET: Booking/marinaowner
        [Route("{controller}/bookingLines")]
        public async Task<IActionResult> BookingsByMarinaOwner()
        {
            var bookingLines = await _bookingLineService.GetAll();
            bookingLines = new List<BookingLine>(bookingLines.ToList().FindAll(bl => bl.StartDate > DateTime.Now.AddDays(2)));

            // User is fully authorized to all content if he is a manager or admin
            var isFullyAuthorized =
                User.IsInRole(RoleName.Administrator) ||
                User.IsInRole(RoleName.Manager);

            // If he is an admin or manager indeed
            if (isFullyAuthorized)
            {
                // Return a view with all the resources displayed
                return View(bookingLines);
            }

            // If user is only a boat owner instead
            if (User.IsInRole(RoleName.MarinaOwner))
            {
                // Get the logged in user's related marina owner object
                var loggedPerson = await _userService.GetUserAsync(User);
                var marinaOwner = _userService.GetMarinaOwnerFromPerson(loggedPerson);

                // Filter results so that they only see their spots in bookings
                bookingLines = await _marinaOwnerService.GetBookingLines(marinaOwner.MarinaOwnerId);
                bookingLines = new List<BookingLine>(bookingLines.ToList().FindAll(bl => bl.StartDate > DateTime.Now.AddDays(2)));

                // Return a view that only displays that marina owner's spots to confirm / cancel
                return View(bookingLines);
            }

            // Forbid access to the page if user is none of the roles of a marinaowner owner, manager or admin
            return Forbid();
        }
    }
}

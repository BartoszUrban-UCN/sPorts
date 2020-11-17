using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Data;
using WebApplication.Models;
using System;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookingController : Controller
    {
        private readonly SportsContext _context;
        private readonly IBookingService _bookingService;
        private readonly IBookingConfirmationService _bookingConfirmationService;

        public BookingController(SportsContext context, IBookingService bookingService, IBookingConfirmationService bookingConfirmationService)
        {
            _context = context;
            _bookingService = bookingService;
            _bookingConfirmationService = bookingConfirmationService;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var sportsContext = _context.Bookings.Include(b => b.Boat);
            return View(await sportsContext.ToListAsync());
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Boat)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            ViewData["BoatId"] = new SelectList(_context.Boats, "BoatId", "BoatId");
            return View();
        }

        // POST: Booking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,BookingReferenceNo,TotalPrice,PaymentStatus,BoatId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoatId"] = new SelectList(_context.Boats, "BoatId", "BoatId", booking.BoatId);
            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["BoatId"] = new SelectList(_context.Boats, "BoatId", "BoatId", booking.BoatId);
            return View(booking);
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingReferenceNo,TotalPrice,PaymentStatus,BoatId")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["BoatId"] = new SelectList(_context.Boats, "BoatId", "BoatId", booking.BoatId);
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Boat)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPut]
        public async Task<IActionResult> ConfirmBookingLine(int bookingLineId)
        {
            var success = await _bookingConfirmationService.ConfirmSpotBooked(bookingLineId);
            return Content(success.ToString());
        }

        /// <summary>
        /// Get Bookings of logged in Marina Owner
        /// </summary>
        /// <returns>View</returns>
        // GET: Booking/marinaowner
        [Route("{controller}/MarinaOwner")]
        public async Task<IActionResult> BookingsByMarinaOwner()
        {

            // get logged in marina owner
            MarinaOwner marinaOwner = _context.MarinaOwners.Where(mo => mo.MarinaOwnerId == 2).FirstOrDefault();
            var bookingLines = await _bookingConfirmationService.GetUnconfirmedBookingLines(marinaOwner.MarinaOwnerId);

            return View(bookingLines);
        }

        [Route("Booking/{id}/GetBookingLines", Name = "blines")]
        public async Task<IActionResult> GetBookingLines(int id)
        {
            try
            {
                var bookingLines = await _bookingService.GetBookingLines(id);
                return View("~/Views/BookingLine/Index.cshtml", bookingLines);
            }
            catch (BusinessException)
            {
                return View("Error");
            }
        }

        [Route("Booking/{id}/Cancel", Name = "cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _bookingService.CancelBooking(id);
            if (success)
            {
                return Content("Canceled!");
            }
            return Content("Not Canceled!");
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}

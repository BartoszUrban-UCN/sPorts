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
    public class BookingController : Controller
    {
        private readonly SportsContext _context;
        private readonly IBookingConfirmationService _bookingConfirmationService;

        public BookingController(SportsContext context)
        {
            _context = context;
            _bookingConfirmationService = new BookingConfirmationService(_context);
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var sportsContext = _context.Bookings.Include(b => b.Boat);
            return View(await sportsContext.ToListAsync());
        }

        [Route("{id}/details")]
        public async Task<IActionResult> Details(int? id)
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

        [HttpPost, ActionName("ConfirmBookingLine")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBookingLine(int bookingLineId)
        {
            var success = await _bookingConfirmationService.ConfirmSpotBooked(bookingLineId);
            return RedirectToAction(nameof(BookingsByMarinaOwner));
        }

        [Route("{controller}/marinaowner/{id}")]
        public async Task<IActionResult> BookingsByMarinaOwner(int marinaOwnerId)
        {

            // get logged in marina owner
            MarinaOwner marinaOwner = _context.MarinaOwners.Where(mo => mo.MarinaOwnerId == 1).FirstOrDefault();
            //var bookingLines = await _bookingConfirmationService.GetUnconfirmedBookingLines(marinaOwner);
            var bookingLines = await _context.BookingLines.ToListAsync();

            bookingLines.ForEach(bl =>
            {
                _context.Entry(bl).Reference(bl => bl.Spot).Load();
                _context.Entry(bl).Reference(bl => bl.Booking).Load();
                _context.Entry(bl.Spot).Reference(s => s.Marina).Load();
                _context.Entry(bl.Spot.Marina).Reference(m => m.Address).Load();
                _context.Entry(bl.Spot.Marina).Reference(m => m.MarinaOwner).Load();
                _context.Entry(bl.Spot.Marina.MarinaOwner).Reference(mo => mo.Person).Load();
                _context.Entry(bl.Booking).Reference(b => b.Boat).Load();
                _context.Entry(bl.Booking.Boat).Reference(b => b.BoatOwner).Load();
                _context.Entry(bl.Booking.Boat.BoatOwner).Reference(bo => bo.Person).Load();
            }
            );

            return View(bookingLines);
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}

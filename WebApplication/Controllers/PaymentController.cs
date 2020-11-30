using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Interfaces;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class PaymentController : Controller
    {
        private readonly SportsContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IBookingService _bookingService;

        public PaymentController(SportsContext context, IPaymentService paymentService, IBookingService bookingService)
        {
            _context = context;
            _paymentService = paymentService;
            _bookingService = bookingService;
        }

        // GET: Payment
        public async Task<IActionResult> Index()
        {
            var sportsContext = _context.Payments.Include(p => p.Booking);
            return View(await sportsContext.ToListAsync());
        }

        // GET: Payment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }
        public async Task<IActionResult> CreateFromBooking()
        {
            //Session 
            var booking = HttpContext.Session.Get<Booking>("Booking");
            if (booking.BookingReferenceNo != 0)
            {
                await _bookingService.SaveBooking(booking);
                //HttpContext.Session.Set("Booking", new Booking());
                HttpContext.Session.Clear();
                var payment = await _paymentService.CreateFromBooking(booking);
                ViewData["BookingId"] = booking.BookingId;
                ViewData["bookingTotalPrice"] = booking.TotalPrice;
                //return View("~/Views/Payment/CreateFromBooking.cshtml", payment);
                return await Create(payment);
            }
            else
            {
                return View();
            }
        }


        // GET: Payment/Create
        public IActionResult Create()
        {
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId");
            return View();
        }

        // POST: Payment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,Amount,BookingId,IncomingPaymentReference,IncomingPaymentStatus,InvoiceStatus")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", payment.BookingId);
            return View(payment);
        }

        // GET: Payment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", payment.BookingId);
            return View(payment);
        }

        // POST: Payment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,Amount,BookingId,IncomingPaymentReference,IncomingPaymentStatus,InvoiceStatus")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", payment.BookingId);
            return View(payment);
        }

        // GET: Payment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }

        public async Task<IActionResult> StartPayment(Payment payment)
        {

            var result = await _paymentService.StartPayment(payment);
            return View(result);
        }
    }
}

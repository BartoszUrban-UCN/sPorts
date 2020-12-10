using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Interfaces;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
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

        /// <summary>
        /// Provides stripe's payment gateway with information about the order
        /// </summary>
        /// <returns></returns>
        [Route("create-payement-session")]
        [HttpPost]
        public async Task<IActionResult> InitiateStripePayment()
        {
            // Load the session booking
            var sessionBooking = HttpContext.Session.Get<Booking>("Booking");

            if (sessionBooking == null)
            {
                HttpContext.Session.Set("Booking", new Booking());
                sessionBooking = HttpContext.Session.Get<Booking>("Booking");
            }

            // Load the session booking's spots for the displaying of information
            sessionBooking = await _bookingService.LoadSpots(sessionBooking);

            // Convert from BookingLines to Stripe's SessionLineItemOptions class
            var sessionLines = ConvertBookingLinesToSessionLines(bookingLines: sessionBooking.BookingLines);

            // Set up Stripe's payment session options
            var options = new SessionCreateOptions
            {
                BillingAddressCollection = "required",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = sessionLines,
                Mode = "payment",
                SuccessUrl = Url.ActionLink("SaveBooking", "BookingFlow"),
                CancelUrl = Url.ActionLink("ShoppingCart", "BookingFlow"),
            };
            var service = new SessionService();
            Session session = service.Create(options);
            return Json(new { id = session.Id });
        }

        // Method name is completely awful please change if you have any better idea
        private List<SessionLineItemOptions> ConvertBookingLinesToSessionLines(IEnumerable<BookingLine> bookingLines)
        {
            var sessionLineItemOptions = new List<SessionLineItemOptions>();

            foreach (BookingLine bookingLine in bookingLines)
            {
                // Price for output
                long price = (long)(bookingLine.DiscountedTotalPrice * 100);

                // Name for output
                string name = $"{bookingLine.Spot?.Marina?.Name} Spot {bookingLine.SpotId}";

                // Description for output
                string description = $"For {(bookingLine.EndDate - bookingLine.StartDate).TotalDays + 1} day";
                if (!bookingLine.StartDate.Equals(bookingLine.EndDate)) description += "s";

                sessionLineItemOptions.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = price,
                        Currency = "dkk",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = name,
                            Description = description,
                        },
                    },
                    Quantity = 1,
                });
            }

            return sessionLineItemOptions.ToList();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Interfaces;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IBookingService _bookingService;

        public PaymentController(SportsContext context, IPaymentService paymentService, IBookingService bookingService)
        {
            _paymentService = paymentService;
            _bookingService = bookingService;
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

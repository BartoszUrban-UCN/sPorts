using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Authorization;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Interfaces;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookingFlowController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IBoatService _boatService;
        private readonly IMarinaService _marinaService;
        private readonly IPaymentService _paymentService;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserService _userService;

        public BookingFlowController(IBookingService bookingService, IBoatService boatService, IMarinaService marinaService, ISpotService spotService, IPaymentService paymentService, IAuthorizationService authorizationService, UserService userService)
        {
            _bookingService = bookingService;
            _boatService = boatService;
            _marinaService = marinaService;
            _paymentService = paymentService;
            _authorizationService = authorizationService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            // User is fully authorized to all content if he is a manager or admin
            var isFullyAuthorized =
                User.IsInRole(RoleName.Administrator) ||
                User.IsInRole(RoleName.Manager);

            // If he is an admin or manager
            if (isFullyAuthorized)
            {
                // Get all the boats in the system as choices of booking
                var boats = await _boatService.GetAll();

                ViewBag.Boat = new SelectList(boats, "BoatId", "Name");
                // Needed for user prompt when deciding to change important values in the booking
                ViewBag.SessionBooking = HttpContext.Session.Get<Booking>("Booking");
                var booking = await _bookingService.CreateEmptyBooking();

                return View(booking);
            }

            // If user is only a boat owner instead
            if (User.IsInRole(RoleName.BoatOwner))
            {
                // Get the logged in user's related boat owner object
                var loggedPerson = await _userService.GetUserAsync(User);
                var boatOwner = _userService.GetBoatOwnerFromPerson(loggedPerson);

                // Filter results so that he only gets his boats rather than all of them
                var boats = (await _boatService.GetAll()).Where(boat => boat.BoatOwnerId == boatOwner.BoatOwnerId);

                ViewBag.Boat = new SelectList(boats, "BoatId", "Name");
                // Needed for user prompt when deciding to change important values in the booking
                ViewBag.SessionBooking = HttpContext.Session.Get<Booking>("Booking");
                var booking = await _bookingService.CreateEmptyBooking();

                return View(booking);
            }

            return Forbid();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost(Booking booking)
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "BoatId", "Name");
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMarinaMap(string boat, string start, string end)
        {
            var boatId = int.Parse(boat);
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            var jsonString = HelperMethods.Serialize(await _bookingService.GetAllAvailableSpotsCount((await _marinaService.GetAll()).Select(m => m.MarinaId).ToList(), boatId, startDate, endDate));

            return new JsonResult(jsonString);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpotMap(string boat, string start, string end, string marina)
        {
            var boatId = int.Parse(boat);
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);
            var marinaId = int.Parse(marina);

            var jsonString = HelperMethods.Serialize(await _bookingService.GetAvailableSpots(marinaId, boatId, startDate, endDate));

            return new JsonResult(jsonString);
        }

        public async Task<IActionResult> ShoppingCart()
        {
            // User is fully authorized to all content if he is a manager or admin
            var isAuthorized =
                User.IsInRole(RoleName.Administrator) ||
                User.IsInRole(RoleName.Manager) || User.IsInRole(RoleName.BoatOwner);

            if (isAuthorized)
            {
                var sessionBooking = HttpContext.Session.Get<Booking>("Booking");

                if (sessionBooking == null)
                {
                    HttpContext.Session.Set("Booking", new Booking());
                    sessionBooking = HttpContext.Session.Get<Booking>("Booking");
                }

                sessionBooking = await _bookingService.LoadSpots(sessionBooking);

                var validBooking = await _bookingService.ValidateShoppingCart(sessionBooking);

                var totalPrice = _bookingService.CalculateTotalPrice(validBooking);
                validBooking.TotalPrice = totalPrice;

                var appliedDiscounts = _bookingService.CalculateTotalDiscount(validBooking);

                var marinaBLineDict = _bookingService.FilterLinesByMarina(validBooking);

                ViewData["MarinaBLineDict"] = marinaBLineDict;
                ViewData["AppliedDiscounts"] = appliedDiscounts;

                sessionBooking = HttpContext.Session.Get<Booking>("Booking");
                byte cartHasChanged = (byte)(validBooking.BookingLines.Count == sessionBooking.BookingLines.Count ? 0 : 1);
                ViewData["CartHasChanged"] = cartHasChanged;
                if (cartHasChanged == 1)
                {
                    HttpContext.Session.Set("Booking", validBooking);
                }

                return View(validBooking);
            }

            return Forbid();
        }

        public async Task<IActionResult> SaveBooking()
        {
            //Session 
            var sessionBooking = HttpContext.Session.Get<Booking>("Booking");
            var booking = new Booking();

            if (sessionBooking != null)
            {
                sessionBooking = await _bookingService.LoadSpots(sessionBooking);
                booking = await _bookingService.ValidateShoppingCart(sessionBooking);
            }

            if (booking.BookingReferenceNo != 0 && booking.BookingLines.Count > 0 && booking.BookingLines.Count == sessionBooking.BookingLines.Count)
            {
                HttpContext.Session.Clear();
                // Ignore exceptions and continue the flow
                try
                {
                    await _bookingService.SaveBooking(booking);
                }
                catch (Exception) { }
                var payment = await _paymentService.CreateFromBooking(booking);
                ViewData["BookingId"] = booking.BookingId;
                ViewData["bookingTotalPrice"] = booking.TotalPrice;

                await _paymentService.Create(payment);
                await _paymentService.Save();

                return RedirectToAction("Index", "Booking");
            }
            else
            {
                return RedirectToAction("ShoppingCart");
            }
        }
    }
}

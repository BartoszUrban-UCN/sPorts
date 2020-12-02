using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class BookingFlowController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IBoatService _boatService;
        private readonly IMarinaService _marinaService;

        public BookingFlowController(IBookingService bookingService, IBoatService boatService, IMarinaService marinaService, ISpotService spotService)
        {
            _bookingService = bookingService;
            _boatService = boatService;
            _marinaService = marinaService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "BoatId", "Name");
            // Needed for user prompt when deciding to change important values
            // in the booking
            ViewBag.SessionBooking = HttpContext.Session.Get<Booking>("Booking");

            var booking = await _bookingService.Create(new Booking { });

            return View(booking);
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
            var sessionBooking = HttpContext.Session.Get<Booking>("Booking");

            if (sessionBooking == null)
            {
                HttpContext.Session.Set("Booking", new Booking());
                sessionBooking = HttpContext.Session.Get<Booking>("Booking");
            }

            sessionBooking = await _bookingService.LoadSpots(sessionBooking);

            var validBooking = await _bookingService.ValidateShoppingCart(sessionBooking);

            var totalPrice = _bookingService.CalculateTotalPrice(validBooking);

            var appliedDiscounts = _bookingService.CalculateTotalDiscount(validBooking);

            var marinaBLineDict = _bookingService.FilterLinesByMarina(validBooking);

            ViewData["MarinaBLineDict"] = marinaBLineDict;
            ViewData["AppliedDiscounts"] = appliedDiscounts;

            return View(validBooking);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.BusinessLogic.Shared;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class BookingFlowController : Controller
    {
        private readonly IBookingFormService _bookingFormService;
        private readonly IBookingService _bookingService;
        private readonly IBoatService _boatService;
        private readonly IMarinaService _marinaService;

        public BookingFlowController(IBookingFormService bookingFormService, IBookingService bookingService, IBoatService boatService, IMarinaService marinaService)
        {
            _bookingFormService = bookingFormService;
            _bookingService = bookingService;
            _boatService = boatService;
            _marinaService = marinaService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Boat = new SelectList(await _boatService.GetAll(), "BoatId", "Name");

            var booking = await _bookingFormService.CreateBooking();
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

            //var availableSpotsPerMarina = await _bookingFormService.GetAvailableSpotsPerMarina(await _marinaService.GetAll(), boatName, startDate, endDate);
            //var jsonString = HelperMethods.Serialize(availableSpotsPerMarina);

            var jsonString = HelperMethods.Serialize(await _marinaService.GetAll());

            return new JsonResult(jsonString);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpotMap(string boat, string start, string end, string marina)
        {
            var boatId = int.Parse(boat);
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);
            var marinaId = int.Parse(marina);

            //var availableSpotsPerMarina = await _bookingFormService.GetAvailableSpotsPerMarina(await _marinaService.GetAll(), boatName, startDate, endDate);
            //var jsonString = HelperMethods.Serialize(availableSpotsPerMarina);

            var jsonString = HelperMethods.Serialize(await _bookingFormService.GetAvailableSpots(marinaId, boatId, startDate, endDate));

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

            await _bookingService.LoadSpots(sessionBooking);

            var validBooking = _bookingService.ValidateShoppingCart(sessionBooking);
            sessionBooking.TotalPrice = Math.Round(sessionBooking.TotalPrice, 2);

            var appliedDiscounts = _bookingService.CalculateTotalDiscount(sessionBooking);
            appliedDiscounts = Math.Round(appliedDiscounts, 2);

            var marinaBLineDict = _bookingService.FilterLinesByMarina(sessionBooking);

            ViewData["MarinaBLineDict"] = marinaBLineDict;
            ViewData["AppliedDiscounts"] = appliedDiscounts;

            return View(sessionBooking);
        }
    }
}
